﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SalesDepartment.Application.Common.Models
{
    public class ResponseCore<T>
    {
        public ResponseCore()
        {

        }
        public ResponseCore(bool isSuccess, string[] Errors)
        {
            this.Errors = Errors;
            this.IsSuccess = isSuccess;
        }
        public ResponseCore(bool isSuccess, T Result)
        {
            IsSuccess = isSuccess;
            this.Result = Result;
        }
        public HttpStatusCode StatusCode { get; set; } = HttpStatusCode.OK;
        public bool IsSuccess { get; set; } = true;
        public string[]? Errors { get; set; }
        public T? Result { get; set; }

    }

}
