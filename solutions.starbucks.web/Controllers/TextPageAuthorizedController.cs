using solutions.starbucks.Interfaces;
using solutions.starbucks.Model;
using solutions.starbucks.Repository.PetaPoco;
using solutions.starbucks.web.Controllers.Masters;
using System.Web.Mvc;

namespace solutions.starbucks.web.Controllers
{
    public class TextPageAuthorizedController : MasterTextPageController
    {
        private readonly ITextPageRepository _textPageRepository;

        public TextPageAuthorizedController(ITextPageRepository textpageRepository)
            : base(_dictionaryRepository)
        {
            _dictionaryRepository = new DictionaryItemRepository();
            _textPageRepository = textpageRepository;
        }

        [Authorize]
        public ActionResult TextPage()
        {
            TextPageModel model = new TextPageModel();
            model = _textPageRepository.GetTextPageProperties(CurrentPage);

            // Handle Redirect
            if (CurrentPage.Name.ToUpper() == "TRAINING" && CurrentPage.Parent.Name.ToUpper() != "SUPPORT")
                return Redirect("~/resources/training/");
            else
                return View(model);
        }
    }
}