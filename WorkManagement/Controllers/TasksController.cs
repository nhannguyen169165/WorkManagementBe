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
    public class TaskModel
    {
        public Models.Task[] taskData;
    }
    [Route("api/[controller]")]
    [ApiController]
    public class TasksController : ControllerBase
    {
        private readonly WorkManagementContext _context;

        public TasksController(WorkManagementContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Route("GetListTasks/{ProjectId}")]
        public async Task<IActionResult> GetListTasks([FromRoute] int ProjectId)
        {
            var listStatus = _context.StatusProject.OrderBy((m) => m.Serial).Where((s) => s.ProjectId == ProjectId);
            var thisProject =  _context.Project.Where((s) => s.Id == ProjectId);
            var projectName = "";
            var listTasks = (from task in _context.Task
                             join status in _context.StatusProject.OrderBy((m) => m.Serial).Where((s) => s.ProjectId == ProjectId)
                             on task.StatusId equals status.Id
                             join user in _context.User
                             on task.TaskOwnerId equals user.Id
                             select new
                             {
                                 TaskId = task.Id,
                                 StatusId = status.Id,
                                 Title = task.Title,
                                 Color = task.Color,
                                 StartDate = task.StartDate,
                                 FinishDate = task.FinishDate,
                                 Priority = task.Priority,
                                 Hours = task.Hours,
                                 TaskOwnerName = user.Fullname
                             });
           
            var strTask = "";
            var strStatus = "";
            foreach (var project in thisProject)
            {
                projectName = project.Name;
            }
            foreach (var status in listStatus)
            {
                strTask = "";
                foreach (var task in listTasks)
                {
                    if(task.StatusId == status.Id)
                    {
                        var taskData = new { id = task.TaskId, title = task.Title, color = task.Color, startDate = task.StartDate, finishDate = task.FinishDate,hours = task.Hours, priority = task.Priority, assignee = task.TaskOwnerName };
                        strTask += JsonConvert.SerializeObject(taskData) + ",";
                    }
                }
                if (strTask == "")
                {
                    strTask = "[]";
                }
                else
                {
                    strTask = strTask.Remove(strTask.Length - 1);
                    strTask = "[" + strTask + "]";
                }
                var statusProjectData = new { statusId = status.Id, statusName = status.StatusName, statusSerial = status.Serial, statusRelation = status.Relation, projectId = status.ProjectId, projectName = projectName, tasks = JsonConvert.DeserializeObject(strTask) };
                strStatus += JsonConvert.SerializeObject(statusProjectData) + ",";
             
            }
            if (strStatus == "")
            {
                return Ok("[]");
            }
            else
            {
                strStatus = strStatus.Remove(strStatus.Length - 1);
                strStatus = "[" + strStatus + "]";
            }
            return Ok(strStatus);
            
        }

      

        // GET: api/Tasks/5
        [HttpGet]
        [Route("GetTaskDetail/{id}")]
        public async Task<IActionResult> GetTaskDetail([FromRoute] int id)
        {
            var task = await _context.Task.SingleOrDefaultAsync(m => m.Id == id);

            var data = new { id = task.Id, title = task.Title, description = task.Description, color = task.Color, startDate = task.StartDate, finishDate = task.FinishDate, hours = task.Hours, priority = task.Priority, taskOwnerId = task.TaskOwnerId , statusId = task.StatusId }; ;
            var str = JsonConvert.SerializeObject(data);
            if (task == null)
            {
                return NotFound();
            }
            else
            {
                return Ok(str);

            }
        }

        [HttpPost]
        [Route("CreateTask")]
        public async Task<IActionResult> CreateTask([FromBody] TaskModel taskModel)
        {

            string str = "Create Task Successfully";
            Models.Task task = new Models.Task();
            foreach (var item in taskModel.taskData)
            {

                task.Title = item.Title;
                task.Description = item.Description;
                task.Color = item.Color;
                task.StartDate = item.StartDate;
                task.FinishDate = item.FinishDate;
                task.Hours = item.Hours;
                task.Priority = item.Priority;
                task.StatusId = item.StatusId;
                task.TaskOwnerId = item.TaskOwnerId;
            }
            _context.Task.Add(task);
            await _context.SaveChangesAsync();
            var result = JsonConvert.SerializeObject(new { result = str });
            return Ok(result);
        }

        [HttpPut]
        [Route("EditTask/{TaskId}")]
        public async Task<IActionResult> UpdateTask([FromBody] TaskModel task, [FromRoute] int TaskId)
        {
            string str = "";
            var thisTask = await _context.Task.SingleOrDefaultAsync(m => m.Id == TaskId);
            foreach (var item in task.taskData)
            {
               
               if(item.StatusId> 0)
               {
                    thisTask.StatusId = item.StatusId;
               }
               if(
                    item.Title != null &&
                    item.Description != null &&
                    item.Color != null &&
                    item.Hours > 0 &&
                    item.Priority != null &&
                    item.TaskOwnerId > 0
               )
               {
                    thisTask.Title = item.Title;
                    thisTask.Description = item.Description;
                    thisTask.Color = item.Color;
                    thisTask.StartDate = item.StartDate;
                    thisTask.FinishDate = item.FinishDate;
                    thisTask.Hours = item.Hours;
                    thisTask.Priority = item.Priority;
                    thisTask.TaskOwnerId = item.TaskOwnerId;
                } 
            }
            _context.Task.Update(thisTask);
            await _context.SaveChangesAsync();
            str = "update task successfully";
            var result = JsonConvert.SerializeObject(new { result = str });
            return Ok(result);
        }


        private bool TaskExists(int id)
        {
            return _context.Task.Any(e => e.Id == id);
        }
    }
}