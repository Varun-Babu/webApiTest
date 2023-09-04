using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace XUnitApi.Models;

public partial class Form
{
    [Key]
    public Guid Id { get; set; }

    [JsonIgnore]
    public Guid? RatebookId { get; set; }

    public Guid? TableId { get; set; }
    [JsonIgnore]
    public string? AddChangeDeleteFlag { get; set; }

    public int? Sequence { get; set; }
    [JsonIgnore]
    public int? SubSequence { get; set; }

    public string? Type { get; set; }

    public int? MinOccurs { get; set; }
   
    public int? MaxOccurs { get; set; }
    [JsonIgnore]
    public string? Number { get; set; }

    public string? Name { get; set; }
    [JsonIgnore]
    public string? Comment { get; set; }
    [JsonIgnore]
    public string? HelpText { get; set; }
    [JsonIgnore]
    public string? Condition { get; set; }
    [JsonIgnore]
    public int? HidePremium { get; set; }
    [JsonIgnore]
    public string? TemplateFile { get; set; }
    [JsonIgnore]
    public int? Hidden { get; set; }
    [JsonIgnore]
    public string? TabCondition { get; set; }
    [JsonIgnore]
    public string? TabResourceName { get; set; }
    [JsonIgnore]
    public string? BtnResAdd { get; set; }
    [JsonIgnore]
    public string? BtnResModify { get; set; }
    [JsonIgnore]
    public string? BtnResDelete { get; set; }
    [JsonIgnore]
    public string? BtnResViewDetail { get; set; }
    [JsonIgnore]
    public string? BtnResRenumber { get; set; }
    [JsonIgnore]
    public string? BtnResView { get; set; }
    [JsonIgnore]
    public string? BtnResCopy { get; set; }
    [JsonIgnore]
    public string? BtnCndAdd { get; set; }
    [JsonIgnore]
    public string? BtnCndModify { get; set; }
    [JsonIgnore]
    public string? BtnCndDelete { get; set; }
    [JsonIgnore]
    public string? BtnCndViewDetail { get; set; }
    [JsonIgnore]
    public string? BtnCndRenumber { get; set; }
    [JsonIgnore]
    public string? BtnCndView { get; set; }
    [JsonIgnore]
    public string? BtnCndCopy { get; set; }
    [JsonIgnore]
    public string? BtnLblAdd { get; set; }
    [JsonIgnore]
    public string? BtnLblModify { get; set; }
    [JsonIgnore]
    public string? BtnLblDelete { get; set; }
    [JsonIgnore]
    public string? BtnLblViewDetail { get; set; }
    [JsonIgnore]
    public string? BtnLblRenumber { get; set; }
    [JsonIgnore]
    public string? BtnLblView { get; set; }

    [JsonIgnore]
    public string? BtnLblCopy { get; set; }

    [JsonIgnore]
    public virtual Aotable? Table { get; set; }
}
