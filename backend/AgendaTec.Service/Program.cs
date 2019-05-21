using System.ServiceProcess;

namespace AgendaTec.Service
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
#if (!DEBUG)
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[]
            {
                new AgendaTecService()
            };
            ServiceBase.Run(ServicesToRun);
#else
            var serv = new AgendaTecService();
            serv.Debug();
#endif      
        }
    }
}
