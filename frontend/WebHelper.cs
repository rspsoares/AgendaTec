using System;
using System.IO;

namespace AgendaTech.View
{
    public class WebHelper
    {
        public bool GravarArquivo(string pasta, string arquivo, MemoryStream mStream, out string msgErro)
        {
            string linkArquivo = string.Empty;
            bool sucesso = false;

            msgErro = string.Empty;

            try 
            {
                Directory.CreateDirectory(pasta);            
                linkArquivo = Path.Combine(pasta, arquivo);
                using (FileStream file = new FileStream(linkArquivo, FileMode.CreateNew, FileAccess.ReadWrite))
                {
                    mStream.WriteTo(file);                 
                    mStream.Close();
                }

                sucesso = true;
            }
            catch (Exception ex)
            {
                msgErro = ex.ToString();
                sucesso = false;
            }

            return sucesso;
        }
    }
}