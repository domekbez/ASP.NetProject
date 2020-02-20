using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static CVEditor.Controllers.HomeController;

namespace CVEditor
{
    public interface IService
    {
        UserType CheckUserType(string nameId);
    }
}
