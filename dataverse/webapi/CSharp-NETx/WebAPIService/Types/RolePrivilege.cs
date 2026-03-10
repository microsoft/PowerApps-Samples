namespace PowerApps.Samples.Types
{
    public class RolePrivilege
    {
        public PrivilegeDepth Depth { get; set; }
        public Guid PrivilegeId { get; set; }
        public Guid BusinessUnitId { get; set; }
        public string PrivilegeName { get; set; }
    }
}
