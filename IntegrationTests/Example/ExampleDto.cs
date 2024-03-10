using System.ComponentModel.DataAnnotations;

namespace Service;

public class CityDto
{
	public int Id { get; set; }

	[Required(AllowEmptyStrings = false, ErrorMessage = "Name is required")]
	public string Name { get; set; } = null!;
}