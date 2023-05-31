using System;
using System.Collections.Generic;

namespace BookRentalShopAPI.Models;

public partial class Usertbl
{
    public int Id { get; set; }

    public string Userid { get; set; } = null!;

    public string Password { get; set; } = null!;
}
