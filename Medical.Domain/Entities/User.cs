using Medical.Domain.Entities.BaseModel;

namespace MedicalMVC.Models;

public class User : Base
{
    public string UserEmail { get; set; }
    public string UserPassword { get; set; }
}