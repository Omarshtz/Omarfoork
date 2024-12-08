﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Models
{
    public class ContactUs
    {
        public int Id { get; set; }
        public string Name { get; set; }
        [Required]
        public RequestType Request { get; set; }
        [Required]
        public string UserRequestString { get; set; }
        [DataType(DataType.PhoneNumber)]
        public string? PhoneNumber { get; set; }
        public string? UserImgRequest { get; set; }

        public enum RequestType
        {
            Complaint,
            Suggestion,
            Company_Request,
        }
    }
}
