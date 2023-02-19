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
    private readonly MemoContext m_context = new MemoContext();

    [HttpGet]
    public IActionResult Get()
    {
        return Ok((m_context.Memos ?? throw new Exception("Unable to get memory from context")).ToList());
    }

    [HttpPost]
    public IActionResult Post([FromBody] Memo? card)
    {
        try
        {
            if (card == null || !ModelState.IsValid)
                return BadRequest(ErrorCode.MemoDetailsRequired.ToString());

            var exist = m_context.Find<Memo>(card.Id);

            if (exist != null)
                return StatusCode(StatusCodes.Status409Conflict, ErrorCode.MemoIdInUse.ToString());

            m_context.Add(card);
            m_context.SaveChanges();
        }
        catch (Exception)
        {
            return BadRequest(ErrorCode.UnableCreateAction.ToString());
        }

        return Ok(card);
    }

    [HttpPut]
    public IActionResult Put([FromBody] Memo? card)
    {
        try
        {
            if (card == null || !ModelState.IsValid)
                return BadRequest(ErrorCode.MemoDetailsRequired.ToString());

            var exist = m_context.Find<Memo>(card.Id);

            if (exist == null)
                return NotFound(ErrorCode.MemoNotFound.ToString());

            exist.Name = card.Name;
            exist.Summary = card.Summary;
            exist.Touch = card.Touch;
            m_context.SaveChanges();
        }
        catch (Exception)
        {
            return BadRequest(ErrorCode.UnableUpdateAction.ToString());
        }

        return Ok(card);
    }

    [HttpDelete("{id:guid}")]
    public IActionResult Delete(Guid id)
    {
        try
        {
            var item = m_context.Find<Memo>(id);

            if (item == null)
                return NotFound(ErrorCode.MemoNotFound.ToString());

            m_context.Remove(item);
            m_context.SaveChanges();
        }
        catch (Exception)
        {
            return BadRequest(ErrorCode.UnableDeleteAction.ToString());
        }

        return NoContent();
    }
}