namespace AuthInASP.NET.Domian
{
    public class Permission : BaseEntity<int>
    {
        public required string Name { get; set; }
        public string? NormalizedName { get; set; }

        public IList<PermissionRole> PermissionRoles { get; set; }
    }
}
