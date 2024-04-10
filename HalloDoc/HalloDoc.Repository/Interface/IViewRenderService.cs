using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDoc.Repository.Interface
{
    public interface IViewRenderService
    {
        public Task<string> RenderToStringAsync(string viewName, object model);
    }
}
