using Microsoft.AspNetCore.Mvc;
using XUnitApi.Models;

namespace XUnitApi.Services
{
    public interface IFormTableRepository
    {
        //get methods
        public Task<List<Form>> GetAllFormsByFormType(string formType);
        Task<(List<Form> forms, string tableName)> GetAllFormsAndTableName(Guid tableId);
        Task<List<Form>> GetFormsByTableName(string tableName);

        //delete
        bool FormExists(Guid id);
        public Task<IActionResult> DeleteForm(Guid id);

        //edit
        public Task<Form> EditForm(string formName, Form editedForm);

        //add
        Task<bool> AddFormForTableName(string tableName, Form form);
        public  Task<Form> AddForm(Form form);




    }
}
