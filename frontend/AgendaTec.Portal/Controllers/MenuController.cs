using AgendaTec.Business.Entities;
using AgendaTec.Portal.Helper;
using System.Text;
using System.Web.Mvc;

namespace AgendaTec.Portal.Controllers
{
    public class MenuController : Controller
    { 
        public string MenuLateral()
        {         
            var menu = new StringBuilder();
            var rootUser = User.GetIsUserRoot();
            var role = (EnUserType)int.Parse(User.GetIdRole());
            
            menu.AppendLine("<li>");
            menu.AppendLine("<a href='/Home' title='Principal'>");
            menu.AppendLine("<span class='glyphicon glyphicon glyphicon-home'/>");
            menu.AppendLine("<span class='item'>Principal</span>");
            menu.AppendLine("</a>");
            menu.AppendLine("</li>");

            menu.AppendLine("<li>");
            menu.AppendLine("<span class='glyphicon glyphicon-edit'/>");
            menu.AppendLine("<span class='nav-title-item'>Cadastros</span>");
            menu.AppendLine("<ul>");

            if (rootUser)
            {
                menu.AppendLine("<li>");
                menu.AppendLine("<a href='/Customers' title='Customer'>");
                menu.AppendLine("<span class='item'>Customer</span>");
                menu.AppendLine("</a>");
                menu.AppendLine("</li>");
            }

            menu.AppendLine("<li>");
            menu.AppendLine("<a href='/Services' title='Serviços'>");
            menu.AppendLine("<span class='item'>Serviços</span>");
            menu.AppendLine("</a>");
            menu.AppendLine("</li>");

            if (role.Equals(EnUserType.Administrator) || role.Equals(EnUserType.Customer))
            {
                menu.AppendLine("<li>");
                menu.AppendLine("<a href='/Users' title='Clientes'>");
                menu.AppendLine("<span class='item'>Clientes</span>");
                menu.AppendLine("</a>");
                menu.AppendLine("</li>");
                menu.AppendLine("</li>");
            }

            menu.AppendLine("<li>");
            menu.AppendLine("<a href='/Professionals' title='Profissionais'>");
            menu.AppendLine("<span class='item'>Profissionais</span>");
            menu.AppendLine("</a>");
            menu.AppendLine("</li>");

            menu.AppendLine("</ul>");

            menu.AppendLine("<li>");
            menu.AppendLine("<span class='glyphicon glyphicon glyphicon-book'/>");
            menu.AppendLine("<span class='item'>Controles</span>");
            menu.AppendLine("<ul>");
            {
                menu.AppendLine("<li>");
                menu.AppendLine("<a href='/Schedules' title='Agenda'>");
                menu.AppendLine("<span class='item'>Agenda</span>");
                menu.AppendLine("</a>");
                menu.AppendLine("</li>");               
            }

            menu.AppendLine("</li>");
            menu.AppendLine("</ul>");

            menu.AppendLine("<li>");
            menu.AppendLine("<span class='glyphicon glyphicon glyphicon-book'/>");
            menu.AppendLine("<span class='item'>Mala Direta</span>");
            menu.AppendLine("<ul>");
            {
                menu.AppendLine("<li>");
                menu.AppendLine("<a href='/DirectMails?MailType=0' title='Mala Direta'>");
                menu.AppendLine("<span class='item'>Geral</span>");
                menu.AppendLine("</a>");
                menu.AppendLine("</li>");

                menu.AppendLine("<li>");
                menu.AppendLine("<a href='/DirectMails?MailType=1' title='Mala Direta'>");
                menu.AppendLine("<span class='item'>E-Mail</span>");
                menu.AppendLine("</a>");
                menu.AppendLine("</li>");

                menu.AppendLine("<li>");
                menu.AppendLine("<a href='/DirectMails?MailType=2' title='Mala Direta'>");
                menu.AppendLine("<span class='item'>WhatsApp</span>");
                menu.AppendLine("</a>");
                menu.AppendLine("</li>");
            }

            menu.AppendLine("</li>");
            menu.AppendLine("</ul>");

            menu.AppendLine("<li>");
            menu.AppendLine("<span class='glyphicon glyphicon glyphicon-th-list'/>");
            menu.AppendLine("<span class='item'>Relatorios</span>");
            menu.AppendLine("<ul>");
            {
                menu.AppendLine("<li>");
                //menu.AppendLine("<a href='/Agendamentos' title='Agendamentos'>");
                menu.AppendLine("<span class='item'>Agendamentos</span>");
                menu.AppendLine("</a>");
                menu.AppendLine("</li>");
            }

            menu.AppendLine("</li>");
            menu.AppendLine("</ul>");
            menu.AppendLine("</li></ul>");

            return menu.ToString();
        }
    }
}