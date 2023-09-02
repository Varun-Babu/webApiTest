using AutoMapper;
using Azure.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Reflection.Metadata;
using XUnitApi.Models;
using XUnitApi.Services;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace XUnitApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FormTableController : ControllerBase
    {
        private readonly IFormTableRepository _formTableRepository;

        public FormTableController(IFormTableRepository formTableRepository)
        {
            _formTableRepository = formTableRepository;

        }
        //Get all records from Form table by passing FormType as parameter
        [HttpGet]
        [Route("{formType}")]
        public async Task<IActionResult> GetAllFormsByFormType([FromRoute] string formType)
        {
            try
            {
                var forms = await _formTableRepository.GetAllFormsByFormType(formType);
                if (forms != null)
                {
                    return Ok(forms);

                }
                return NotFound("No form existing with the given name");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("/forms/{tableId}")]
        public async Task<IActionResult> GetFormsAndTableName(Guid tableId)
        {
            try
            {
                var (forms, tableName) = await _formTableRepository.GetAllFormsAndTableName(tableId);

                if (forms == null || forms.Count == 0)
                {
                    return NotFound();
                }
                var result = new
                {
                    TableName = tableName,
                    Forms = forms
                };
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    
      
        

        [HttpGet("/form/{tableName}")]
        public async Task<IActionResult> GetFormsByTableName(string tableName)
        {
            try
            {
                var forms = await _formTableRepository.GetFormsByTableName(tableName);

                if (forms == null || forms.Count == 0)
                {
                    return NotFound();
                }

                return Ok(forms);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteForm(Guid id)
        {
            try
            {
                var exist =  _formTableRepository.FormExists(id);
                if (!exist)
                {
                    return NotFound("Form not found");
                }
                var deleted = await _formTableRepository.DeleteForm(id);
                if (deleted == null)
                {
                    return Ok("Form Deleted");
                }
                return BadRequest("Something went wrong");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

       

        [HttpPut]
        public async Task<IActionResult> EditForm(string formName, Form form)
        {
            try
            {
                var existingForm = await _formTableRepository.EditForm(formName, form);
                if (existingForm != null)
                {
                    return Ok("Successfully Updated");

                }
                return NotFound("No Such Form Exists");

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /*

        [HttpPost("/add/{tableName}")]
        public async Task<IActionResult> AddFormForTable(string tableName, Form form)
        {
            bool added = await _formTableRepository.AddFormForTableName(tableName, form);

            if (added)
            {
                return Ok("Form added successfully.");
            }
            else
            {
                return NotFound("Aotable record not found for the specified table name.");
            }

        } */


        [HttpPost]
        public async Task<IActionResult> AddForm([FromBody] Form form)
        {
            try
            {
                if (form == null)
                {
                    return BadRequest("Invalid form data.");
                }

                var addedForm = await _formTableRepository.AddForm(form);

                if (addedForm != null)
                {
                    return Ok(addedForm);
                }
                else
                {
                    return NotFound("name not available in AOtable");
                }
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}










    

    
