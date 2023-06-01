
namespace AuthInASP.NET.Endpoints.Models
{
    public class SendCodeModel
    {
        public string SelectedProvider { get; set; }

        public ICollection<SelectListItem> Providers { get; set; }

    }
}
