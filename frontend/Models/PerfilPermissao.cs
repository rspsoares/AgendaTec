using System.ComponentModel.DataAnnotations;

namespace AgendaTech.View.Models
{
    public class PerfilPermissao
    {
        [Display(Name = "Perfil")]
        public string Perfil { get; set; }

        public int Identificador { get; set; }
    }
}