using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using QuickFix;
using System.Data;
using QuickFix.DataDictionary;
using QuickFix.Fields;

namespace FIXAcceptor
{
    public class TestCracker : MessageCracker
    {
        public bool CrackedNews42 { get; set; }
        public bool CrackedNews44 { get; set; }

        public void OnMessage(QuickFix.FIX42.News msg, SessionID s) { CrackedNews42 = true; }
        public void OnMessage(QuickFix.FIX44.News msg, SessionID s) { CrackedNews44 = true; }

        public void OnMessage(QuickFix.FIX50.TradeCaptureReport msg, SessionID s)
        {
        }
    }  

    public class Program
    {


        [STAThread]
        static void Main(string[] args)
        {

            SessionID _DummySessionID = new SessionID("a","b","c");
   
            IMessageFactory _defaultMsgFactory = new DefaultMessageFactory();
            
            //string data = "8=FIXT.1.1"+Message.SOH+"9=735"+Message.SOH+"35=AE"+Message.SOH+"34=494"+Message.SOH+"1128=8"+Message.SOH+"49=RTNSFIXUAT"+Message.SOH+"56=YEPIRECUAT"+Message.SOH+"52=20151027-11:59:15"+Message.SOH+"552=1"+Message.SOH+"54=1"+Message.SOH+"37=Z0009WSGC4"+Message.SOH+"11=NOREF"+Message.SOH+"826=0"+Message.SOH+"78=1"+Message.SOH+"79=NOT SPECIFIED"+Message.SOH+"80=120.000000"+Message.SOH+"5967=91.070000"+Message.SOH+"5966=120.000000"+Message.SOH+"5968=91.040000"+Message.SOH+"453=5"+Message.SOH+"448=YPKB"+Message.SOH+"452=3"+Message.SOH+"447=D"+Message.SOH+"802=1"+Message.SOH+"523=YPKB"+Message.SOH+"803=5"+Message.SOH+"448=BARX"+Message.SOH+"452=1"+Message.SOH+"447=D"+Message.SOH+"802=1"+Message.SOH+"523=BARX"+Message.SOH+"803=5"+Message.SOH+"448=BARX"+Message.SOH+"452=16"+Message.SOH+"447=D"+Message.SOH+"802=1"+Message.SOH+"523=BARX"+Message.SOH+"803=5"+Message.SOH+"448=bcart7"+Message.SOH+"452=11"+Message.SOH+"447=D"+Message.SOH+"448=Barclays Capital"+Message.SOH+"452=12"+Message.SOH+"447=D"+Message.SOH+"571=10106232"+Message.SOH+"150=F"+Message.SOH+"17=Z0009WSGC4"+Message.SOH+"32=120.000000"+Message.SOH+"38=120.000000"+Message.SOH+"15=CAD"+Message.SOH+"31=1.317700"+Message.SOH+"555=2"+Message.SOH+"624=1"+Message.SOH+"637=1.317700"+Message.SOH+"1418=120.000000"+Message.SOH+"588=20151028"+Message.SOH+"587=0"+Message.SOH+"1073=0.0000"+Message.SOH+"1074=91.070000"+Message.SOH+"5190=1.317700"+Message.SOH+"624=2"+Message.SOH+"637=1.318062"+Message.SOH+"1418=120.000000"+Message.SOH+"588=20151130"+Message.SOH+"587=M1"+Message.SOH+"1073=3.6200"+Message.SOH+"1074=91.040000"+Message.SOH+"5190=1.317700"+Message.SOH+"60=20151027-11:59:14"+Message.SOH+"75=20151027"+Message.SOH+"1057=Y"+Message.SOH+"39=2"+Message.SOH+"460=4"+Message.SOH+"167=FOR"+Message.SOH+"65=SW"+Message.SOH+"55=USD/CAD"+Message.SOH+"10=076"+Message.SOH;
            
            //var msg = new QuickFix.FIX50.TradeCaptureReport();
            //var dd = new QuickFix.DataDictionary.DataDictionary();
            //dd.Load(@"C:\Code\tfs\neerajkaushik\MarketConnect Gateway\Source\Source\QuickFix1.5\quickfixn-master\quickfixn-master\spec\fix\reuters.fix50sp1.xml");
            //msg.FromString(data, false, dd, dd, _defaultMsgFactory);

            //MessageCracker mc = new TestCracker();

            //mc.Crack(msg, _DummySessionID);

            //var grp = msg.GetGroup(1, Tags.NoLegs);



            Console.WriteLine("=============");
            Console.WriteLine("This is only an example program.");
            Console.WriteLine("It's a simple server (e.g. Acceptor) app that will let clients (e.g. Initiators)");
            Console.WriteLine("connect to it.  It will accept and display any application-level messages that it receives.");
            Console.WriteLine("Connecting clients should set TargetCompID to 'SIMPLE' and SenderCompID to 'CLIENT1' or 'CLIENT2'.");
            Console.WriteLine("Port is 5001.");
            Console.WriteLine("(see simpleacc.cfg for configuration details)");
            Console.WriteLine("=============");
                             


            try
            {
                SessionSettings settings = new SessionSettings("acceptor.cfg");
                Application executorApp = new Executor();
                MessageStoreFactory storeFactory = new FileStoreFactory(settings);
                LogFactory logFactory = new FileLogFactory(settings);
                ThreadedSocketAcceptor acceptor = new ThreadedSocketAcceptor(executorApp, storeFactory, settings, logFactory);

                acceptor.Start();
                Console.WriteLine("press <enter> to quit");
                Console.Read();
                acceptor.Stop();
            }
            catch (System.Exception e)
            {
                Console.WriteLine("==FATAL ERROR==");
                Console.WriteLine(e.ToString());
                Console.WriteLine("press <enter> to quit");
                Console.Read();
            }
        }
    }
    public class SimpleAcceptorApp : /*QuickFix.MessageCracker,*/ QuickFix.Application
    {
        #region QuickFix.Application Methods

        public void FromApp(Message message, SessionID sessionID)
        {
            _currentSessionId = sessionID;
            Console.WriteLine("IN:  " + message);

            string msgtype = message.Header.GetString(35);

            if (msgtype == "V")
            {
                //   QuickFix.Message msg = new Message(message.ToString());
                //  Session.SendToTarget(msg, sessionID);
                var th = new Thread(StartSendingMarketData) { IsBackground = true };
                th.Start();
            }

        }

        private SessionID _currentSessionId;

        private void StartSendingMarketData()
        {
            StreamReader reader = new StreamReader(@"C:\Code\NigSEApplication\FIXT.1.1-PROD-XTRM.messages.current\log1.log");

            DefaultMessageFactory defaultMessageFactory = new DefaultMessageFactory();

            QuickFix.Message msg = new QuickFix.Message();

            while (!reader.EndOfStream)
            {

                string line = reader.ReadLine();

                string txt = "8=FIXT.1.19=00023635=W49=XTRM56=PROD34=452=20140820-07:26:00.726264=175=20140820262=*_148=MOBIL22=99762=EQTY268=5269=a326=1764=20140825269=s270=175.0000000269=u270=175.0000000269=x270=175.0000000269=y332=182.0000000333=106.000000010=143";


                // line.Substring(line.IndexOf("8=") - 1).Trim();

                msg.FromString(txt, false, null, null);
                string msgtype = msg.Header.GetString(35);

                if (msgtype == "W") //|| msgtype == "X")
                {
                    QuickFix.Message newmsg = new Message(txt);
                    if (Session.LookupSession(_currentSessionId).IsLoggedOn)
                    {
                        Session.LookupSession(_currentSessionId).ValidateLengthAndChecksum = false;
                        Session.SendToTarget(newmsg, _currentSessionId);
                    }
                    else
                    {
                        break;
                    }
                }
                Thread.Sleep(100);

            }
            reader.Close();
        }
        public void ToApp(Message message, SessionID sessionID)
        {
            Console.WriteLine("OUT: " + message);
        }

        public void FromAdmin(Message message, SessionID sessionID)
        {
            string msgtype = message.Header.GetString(35);
            if (msgtype == "A")
            {
                // send logon

                //  Session.SendToTarget(message, sessionID);
            }
            Console.WriteLine("IN:  " + message);
        }

        public void ToAdmin(Message message, SessionID sessionID)
        {
            Console.WriteLine("OUT:  " + message);
        }

        public void OnCreate(SessionID sessionID) { }

        public void OnLogout(SessionID sessionID)
        {

        }

        public void OnLogon(SessionID sessionID)
        {

        }
        #endregion
    }
}
