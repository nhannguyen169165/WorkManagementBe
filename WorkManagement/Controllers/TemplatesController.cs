using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using WorkManagement.Models;

namespace WorkManagement.Controllers
{

    public class TemplateModel {
        public Template[] templateData { get; set; }
    }

    public class StatusModel
    {
        public StatusTemplate[] statusData { get; set; }
    }
    [Route("api/[controller]")]
    [ApiController]
    public class TemplatesController : ControllerBase
    {
        private readonly WorkManagementContext _context;

        public TemplatesController(WorkManagementContext context)
        {
            _context = context;
        }

        // GET: api/Templates
        [HttpGet,Authorize]
        [Route("GetTemplate/{AdminId}")]
        public async Task<IActionResult> GetTemplate([FromRoute] int AdminId)
        {
            var template = _context.Template.Where((temp) => temp.AdminId == AdminId);
            string str = "";
            foreach (var item in template)
            {
               
                        var data = new { id = item.Id, name = item.TemplateName};
                        str += JsonConvert.SerializeObject(data) + ",";
                    
            }
            str = str.Remove(str.Length - 1);
            str = "[" + str + "]";
            if (template == null)
            {    
               return Ok("[]");
            }
            else
            {
                return Ok(str);

            }
        }

        [HttpGet("{id}"), Authorize(Roles = "Admin")]
        [Route("GetTemplateDetail/{id}")]
        public async Task<IActionResult> GetTemplateDetail([FromRoute] int id)
        {
            var template = await _context.Template.SingleOrDefaultAsync(m => m.Id == id);
            var data = new { id = template.Id, templateName = template.TemplateName};
            var str = JsonConvert.SerializeObject(data);
            if (template == null)
            {
                return NotFound();
            }
            else
            {
                return Ok(str);

            }
        }


        // GET: api/Templates/5
        [HttpGet, Authorize]
        [Route("GetStatusTemplate/{TemplateId},{AdminId}")]
        public async Task<IActionResult> GetStatusTemplate([FromRoute] int TemplateId,[FromRoute] int AdminId)
        {

            var template = (from temp in _context.Template
                              join stTemp in _context.StatusTemplate.OrderBy((m)=> m.Serial)
                              on temp.Id equals stTemp.TemplateId
                              select new
                              {
                                  Id = temp.Id,
                                  TemplateName = temp.TemplateName,
                                  AdminId = temp.AdminId,
                                  StatusId = stTemp.Id,
                                  StatusName = stTemp.StatusName,
                                  Serial = stTemp.Serial,
                                  Relation = stTemp.Relation
                              });
            if (template == null)
            {
                return NotFound();
            }
            else
            {
                string str = "";
                foreach (var item in template)
                {
                    if (item.Id == TemplateId && item.AdminId == AdminId)
                    {
                        var data = new { id = item.Id, templateName = item.TemplateName, statusId = item.StatusId, statusName = item.StatusName, serial = item.Serial,relation = item.Relation };
                        str += JsonConvert.SerializeObject(data) + ",";
                    }
                }
                if (str == "")
                {
                    return Ok("[]");
                }
                str = str.Remove(str.Length - 1);
                str = "[" + str + "]";
                return Ok(str);
            }
        }


        // POST: api/Templates
        [HttpPost, Authorize(Roles = "Admin")]
        [Route("CreateTemplate")]
        public async Task<IActionResult> CreateTemplate([FromBody] TemplateModel template)
        {

            string str = "Create Successfully";
            Template temp = new Template();
            foreach (var item in template.templateData)
            {

                temp.TemplateName = item.TemplateName;
                temp.AdminId = item.AdminId;
            }
            _context.Template.Add(temp);
            await _context.SaveChangesAsync();
            var result = JsonConvert.SerializeObject(new { result = str });
            return Ok(result);
        }

        [HttpPost, Authorize(Roles = "Admin")]
        [Route("AddNewStatus")]
        public async Task<IActionResult> AddNewStatus([FromBody] StatusModel status)
        {

            string str = "Create Status Template Successfully";
            StatusTemplate statusTemp = new StatusTemplate();
            foreach (var item in status.statusData)
            {

                statusTemp.StatusName = item.StatusName;
                statusTemp.Serial= item.Serial;
                statusTemp.TemplateId = item.TemplateId;
                statusTemp.Relation = 0;
            }
            _context.StatusTemplate.Add(statusTemp);
            await _context.SaveChangesAsync();
            var result = JsonConvert.SerializeObject(new { result = str });
            return Ok(result);
        }

        // PUT: api/Projects/5
        [HttpPut, Authorize(Roles = "Admin")]
        [Route("EditTemplate/{id}")]
        public async Task<IActionResult> UpdateTemplate([FromRoute] int id, [FromBody] TemplateModel template)
        {
            string str = "";
            var thisTemplate = await _context.Template.SingleOrDefaultAsync(m => m.Id == id);
            foreach (var item in template.templateData)
            {
                thisTemplate.TemplateName = item.TemplateName; 
            }

            _context.Template.Update(thisTemplate);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TemplateExists(id))
                {
                    return NotFound();
                }
                else
                {
                    str = "Updated successfully";
                }
            }
            var result = JsonConvert.SerializeObject(new { result = str });
            return Ok(result);
        }

        // PUT: api/Projects/5
        [HttpPut, Authorize(Roles = "Admin")]
        [Route("EditStatusTemplate")]
        public async Task<IActionResult> UpdateStatusTemplate([FromBody] StatusModel status)
        {
            string str = "";
    
            foreach (var item in status.statusData)
            {
                var thisStatusTemplate = await _context.StatusTemplate.SingleOrDefaultAsync(m => m.Id == item.Id);
                if (item.StatusName == null && item.Serial > 0)
                {
                    thisStatusTemplate.Serial = item.Serial;
                }
                else if (item.StatusName != null && item.Serial == 0)
                {
                    thisStatusTemplate.StatusName = item.StatusName;
                }
                else
                {
                    thisStatusTemplate.Relation = item.Relation;
                }
                    _context.StatusTemplate.Update(thisStatusTemplate);
            }
            await _context.SaveChangesAsync();
            str = "update successfully";
            var result = JsonConvert.SerializeObject(new { result = str });
            return Ok(result);
        }

        // DELETE: api/Templates/5
        [HttpDelete, Authorize(Roles = "Admin")]
        [Route("DeleteTemplate/{id}")]
        public async Task<IActionResult> DeleteTemplate([FromRoute] int id)
        {
            var template = await _context.Template.FirstOrDefaultAsync(m => m.Id == id);
            string str = "Xóa thành công";
            var result = JsonConvert.SerializeObject(new { result = str });
            if (template == null)
            {
                return NotFound();
            }
            else
            {
                _context.Template.Remove(template);
                await _context.SaveChangesAsync();
                return Ok(result);
            }


        }

        // DELETE: api/Templates/5
        [HttpDelete, Authorize(Roles = "Admin")]
        [Route("DeleteStatus/{id}")]
        public async Task<IActionResult> DeleteStatus([FromRoute] int id)
        {
            var statusTemplate = await _context.StatusTemplate.FirstOrDefaultAsync(m => m.Id == id);
        
            string str = "Xóa thành công";
            var result = JsonConvert.SerializeObject(new { result = str });
            if (statusTemplate == null)
            {
                return NotFound();
            }
            else
            {
                _context.StatusTemplate.Remove(statusTemplate);
              
                await _context.SaveChangesAsync();
                return Ok(result);
            }


        }
    
        private bool TemplateExists(int id)
        {
            return _context.Template.Any(e => e.Id == id);
        }
    }

}