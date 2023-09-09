using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalesDepartment.Application.Common.Models
{
    public record ExcelReportResponse(byte[] FileContents, string Option, string FileName);
}
