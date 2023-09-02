using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace XUnitApi.Models;

public partial class Form
{
    [Key]
    public Guid Id { get; set; }

    public Guid? RatebookId { get; set; }

    public Guid? TableId { get; set; }

    public string? AddChangeDeleteFlag { get; set; }

    public int? Sequence { get; set; }

    public int? SubSequence { get; set; }

    public string? Type { get; set; }

    public int? MinOccurs { get; set; }

    public int? MaxOccurs { get; set; }

    public string? Number { get; set; }

    public string? Name { get; set; }

    public string? Comment { get; set; }

    public string? HelpText { get; set; }

    public string? Condition { get; set; }

    public int? HidePremium { get; set; }

    public string? TemplateFile { get; set; }

    public int? Hidden { get; set; }

    public string? TabCondition { get; set; }

    public string? TabResourceName { get; set; }

    public string? BtnResAdd { get; set; }

    public string? BtnResModify { get; set; }

    public string? BtnResDelete { get; set; }

    public string? BtnResViewDetail { get; set; }

    public string? BtnResRenumber { get; set; }

    public string? BtnResView { get; set; }

    public string? BtnResCopy { get; set; }

    public string? BtnCndAdd { get; set; }

    public string? BtnCndModify { get; set; }

    public string? BtnCndDelete { get; set; }

    public string? BtnCndViewDetail { get; set; }

    public string? BtnCndRenumber { get; set; }

    public string? BtnCndView { get; set; }

    public string? BtnCndCopy { get; set; }

    public string? BtnLblAdd { get; set; }

    public string? BtnLblModify { get; set; }

    public string? BtnLblDelete { get; set; }

    public string? BtnLblViewDetail { get; set; }

    public string? BtnLblRenumber { get; set; }

    public string? BtnLblView { get; set; }

    public string? BtnLblCopy { get; set; }

    [JsonIgnore]
    public virtual Aotable? Table { get; set; }
}
