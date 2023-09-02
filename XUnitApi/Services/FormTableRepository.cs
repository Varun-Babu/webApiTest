using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using XUnitApi.Data;
using XUnitApi.Models;

namespace XUnitApi.Services
{
    public class FormTableRepository : IFormTableRepository
    {
        private readonly ApiDbContext apiDbContext;

        public FormTableRepository(ApiDbContext apiDbContext)
        {
            this.apiDbContext = apiDbContext;
        }

        public async Task<List<Form>> GetAllFormsByFormType(string formType)
        {
            var forms = await apiDbContext.Forms
                .Where(x => x.Type == formType)
                .ToListAsync();
            return forms;
        }

        public async Task<(List<Form> forms, string tableName)> GetAllFormsAndTableName(Guid tableId)
        {
            var forms = await apiDbContext.Forms
                .Where(f => f.TableId == tableId)
                .ToListAsync();

            var tableName = await apiDbContext.Aotables
                .Where(a => a.Id == tableId)
                .Select(a => a.Name)
                .FirstOrDefaultAsync();
            return (forms,tableName);
        }

        public async Task<List<Form>> GetFormsByTableName(string tableName)
        {
            return await apiDbContext.Forms
                .Where(f => f.Table.Name == tableName)
                .ToListAsync();
        }

        public bool FormExists(Guid id)
        {
            return apiDbContext.Forms.Any(f => f.Id == id);
            
        }

        public async Task<IActionResult> DeleteForm(Guid id)
        {
            var form = apiDbContext.Forms.Where(f => f.Id == id).FirstOrDefault();
            apiDbContext.Forms.Remove(form);
            await apiDbContext.SaveChangesAsync();
            return null;
        }

        public async Task<Form> EditForm(string formName, Form editedForm)
        {
            var formexist = apiDbContext.Forms.Where(f => f.Name == formName).FirstOrDefault();
            if(formexist != null)
            {
                if (editedForm.Name != null)
                {
                    formexist.Name = editedForm.Name;
                }
                if (editedForm.Sequence != null)
                {
                    formexist.Sequence = editedForm.Sequence;
                }
                if (editedForm.Number != null)
                {
                    formexist.Number = editedForm.Number;
                }
                await apiDbContext.SaveChangesAsync();
                return formexist;
            }
            return null;
        }



        public async Task<bool> AddFormForTableName(string tableName, Form form)
        {

            var aotable = await apiDbContext.Aotables.FirstOrDefaultAsync(t => t.Name == tableName);
            if (aotable != null)
            {
                form.TableId = aotable.Id;
                apiDbContext.Forms.Add(form);
                await apiDbContext.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task<Form> AddForm(Form form)
        {
            var nameExist = apiDbContext.Aotables.FirstOrDefault(n => n.Id == form.TableId && n.Name != null);
            if (nameExist != null)
            {
                apiDbContext.Forms.Add(form);
                await apiDbContext.SaveChangesAsync();
                return form;
            }
            return null;
        }
        
    }
}


