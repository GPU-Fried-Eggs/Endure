using Endure.Server.Data;
using Endure.Server.Errors;
using Endure.Server.Models;
using Microsoft.AspNetCore.Mvc;

namespace Endure.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MemoController : ControllerBase
{
    private readonly AppDbContext m_dbContext;

    private readonly ILogger<MemoController> m_logger;

    public MemoController(AppDbContext context, ILogger<MemoController> logger)
    {
        m_dbContext = context;
        m_logger = logger;
    }

    [HttpGet]
    public IActionResult Get()
    {
        try
        {
            return Ok((m_dbContext.Memos ?? throw new Exception("Unable to get memory from context")).ToList());
        }
        catch (Exception e)
        {
            m_logger.Log(LogLevel.Error, e.ToString());
            return BadRequest(ErrorCode.UnableCreateAction.ToString());
        }
    }

    [HttpPost]
    public IActionResult Post([FromBody] Memo? memo)
    {
        try
        {
            if (memo == null || !ModelState.IsValid)
                return BadRequest(ErrorCode.MemoDetailsRequired.ToString());

            var exist = m_dbContext.Find<Memo>(memo.MemoId);

            if (exist != null)
                return StatusCode(StatusCodes.Status409Conflict, ErrorCode.MemoIdInUse.ToString());

            m_dbContext.Memos?.Add(memo);
            m_dbContext.SaveChanges();
        }
        catch (Exception e)
        {
            m_logger.Log(LogLevel.Error, e.ToString());
            return BadRequest(ErrorCode.UnableCreateAction.ToString());
        }

        return Ok(memo);
    }

    [HttpPut]
    public IActionResult Put([FromBody] Memo? memo)
    {
        try
        {
            if (memo == null || !ModelState.IsValid)
                return BadRequest(ErrorCode.MemoDetailsRequired.ToString());

            var exist = m_dbContext.Find<Memo>(memo.MemoId);

            if (exist == null)
                return NotFound(ErrorCode.MemoNotFound.ToString());

            exist.Name = memo.Name;
            exist.Summary = memo.Summary;
            exist.Level = memo.Level;
            m_dbContext.SaveChanges();
        }
        catch (Exception e)
        {
            m_logger.Log(LogLevel.Error, e.ToString());
            return BadRequest(ErrorCode.UnableUpdateAction.ToString());
        }

        return Ok(memo);
    }

    [HttpDelete("{id:guid}")]
    public IActionResult Delete(Guid id)
    {
        try
        {
            var item = m_dbContext.Find<Memo>(id);

            if (item == null)
                return NotFound(ErrorCode.MemoNotFound.ToString());

            m_dbContext.Memos?.Remove(item);
            m_dbContext.SaveChanges();
        }
        catch (Exception e)
        {
            m_logger.Log(LogLevel.Error, e.ToString());
            return BadRequest(ErrorCode.UnableDeleteAction.ToString());
        }

        return NoContent();
    }
}