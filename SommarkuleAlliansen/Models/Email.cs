using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SommarkuleAlliansen.Models
{
    public class Email
    {
        [Required(ErrorMessage = "Du måste ange ditt namn.")]
        public string FromName { get; set; }
        [Required(ErrorMessage = "Du måste ange din epostadress.")]
        public string FromEmail { get; set; }
        [Required(ErrorMessage = "Du måste ange ett ämne.")]
        public string Subject { get; set; }
        [Required(ErrorMessage = "Du måste ange ett meddelande.")]
        public string Message { get; set; }
    }
}