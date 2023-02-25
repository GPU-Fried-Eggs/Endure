using Endure.Server.Data;
using Endure.Server.Models;
using Microsoft.AspNetCore.Mvc;

namespace Endure.Server.Controllers;

public enum ErrorCode
{
    MemoDetailsRequired,
    MemoIdInUse,
    MemoNotFound,
    UnableCreateAction,
    UnableUpdateAction,
    UnableDeleteAction
}

[ApiController]
[Route("api/[controller]")]
public class MemoController : ControllerBase
{
    private readonly EndureDbContext m_dbContext = new EndureDbContext("");

    [HttpGet]
    public IActionResult Get()
    {
        return Ok((m_dbContext.Memos ?? throw new Exception("Unable to get memory from context")).ToList());
    }

    [HttpPost]
    public IActionResult Post([FromBody] Memo? memo)
    {
        try
        {
            if (memo == null || !ModelState.IsValid)
                return BadRequest(ErrorCode.MemoDetailsRequired.ToString());

            var exist = m_dbContext.Find<Memo>(memo.Id);

            if (exist != null)
                return StatusCode(StatusCodes.Status409Conflict, ErrorCode.MemoIdInUse.ToString());

            m_dbContext.Add(memo);
            m_dbContext.SaveChanges();
        }
        catch (Exception)
        {
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

            var exist = m_dbContext.Find<Memo>(memo.Id);

            if (exist == null)
                return NotFound(ErrorCode.MemoNotFound.ToString());

            exist.Name = memo.Name;
            exist.Summary = memo.Summary;
            exist.Touch = memo.Touch;
            m_dbContext.SaveChanges();
        }
        catch (Exception)
        {
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

            m_dbContext.Remove(item);
            m_dbContext.SaveChanges();
        }
        catch (Exception)
        {
            return BadRequest(ErrorCode.UnableDeleteAction.ToString());
        }

        return NoContent();
    }
}