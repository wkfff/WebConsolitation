using System.Web.Mvc;

namespace Krista.FM.RIA.Extensions.E86N.Presentation.Controllers.RestControllers
{
    public interface IRestController
    {
        ActionResult Index(int? parentId);

        ActionResult Read(int itemId);

        [HttpPost]
        ActionResult Create(string data);

        [HttpPut]
        ActionResult Update(string data);

        [HttpDelete]
        ActionResult Delete(int id);
    }
}
