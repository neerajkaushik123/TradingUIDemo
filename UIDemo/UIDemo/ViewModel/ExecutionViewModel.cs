using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UIDemo.Model;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows.Threading;
using FIXApplication;

namespace UIDemo.ViewModel
{
    public class ExecutionViewModel : ViewModelBase
    {
        private QFApp _qfapp = null;

        private Object _executionsLock = new Object();
        public ObservableCollection<ExecutionRecord> Executions { get; set; }

        public ExecutionViewModel(QFApp app)
        {
            _qfapp = app;
            Executions = new ObservableCollection<ExecutionRecord>();

            _qfapp.Fix44ExecReportEvent += new Action<QuickFix.FIX44.ExecutionReport>(HandleExecutionReport);
        }

        public void HandleExecutionReport(QuickFix.FIX44.ExecutionReport msg)
        {
            try
            {
                string execId = msg.ExecID.Obj;
                //string transType = FixEnumTranslator.Translate(msg.ExecType);
                string execType = FixEnumTranslator.Translate(msg.ExecType);

                Trace.WriteLine("EVM: Handling ExecutionReport: " + execId + " / " + execType);

                ExecutionRecord exRec = new ExecutionRecord(
                    msg.ExecID.Obj,
                    msg.OrderID.Obj,
                    string.Empty,
                    execType,
                    msg.Symbol.Obj,
                    FixEnumTranslator.Translate(msg.Side));

                exRec.LeavesQty = msg.LeavesQty.getValue();
                exRec.TotalFilledQty = msg.CumQty.getValue();
                exRec.LastQty = msg.LastQty.getValue();
                
                SmartDispatcher.Invoke(new Action<ExecutionRecord>(AddExecution), exRec);
            }
            catch (Exception e)
            {
                Trace.WriteLine(e.ToString());
            }
        }

        public void AddExecution(ExecutionRecord r)
        {
            try
            {
                Trace.WriteLine("add execution");
                Executions.Add(r);
            }
            catch (Exception e)
            {
                Trace.WriteLine(e.ToString());
            }
        }
    }
}
