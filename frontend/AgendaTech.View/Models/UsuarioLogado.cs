using AgendaTech.Business.Entities;
using System;

namespace AgendaTech.View.Models
{
    public class UsuarioLogado
    {
        public int idUsuario { get; set; }
        public string Login { get; set; }
        public string Nome { get; set; }
        public Guid uqUsuario { get; set; }
        public string Email { get; set; }       
        public EnUserType Inscricao { get; set; }
        public int IDCustomer { get; set; }
    }   
}
