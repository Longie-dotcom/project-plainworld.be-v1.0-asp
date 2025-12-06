using Application.Enum;
using Domain.Aggregate;
using Domain.Entity;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Seed
{
    public static class RolePrivilegeSeeder
    {
        public static async Task SeedAsync(IAMDBContext context)
        {
            // Flag
            if (await context.Roles.AnyAsync(r => r.Code == RoleKey.ADMIN))
            {
                Console.WriteLine("DATABASE HAS BEEN SEEDED ALREADY");
                return;
            }

            // -----------------------------
            // 1️ Seed Roles (constructor used)
            // -----------------------------
            var rolesToSeed = new List<Role>
            {
                new Role(Guid.NewGuid(), "Super Administrator", RoleKey.SUPER_ADMIN, "Full access to all system features."),
                new Role(Guid.NewGuid(), "Administrator", RoleKey.ADMIN, "Full access to all system features but has some limitation compare to Super Administrator."),
                new Role(Guid.NewGuid(), "Lab Manager", RoleKey.LAB_MANAGER, "Manages laboratory operations and system configurations."),
                new Role(Guid.NewGuid(), "Service", RoleKey.SERVICE, "Performs system maintenance, operations, and monitoring tasks."),
                new Role(Guid.NewGuid(), "Lab User", RoleKey.LAB_USER, "Conducts lab tests, analyzes samples, and manages lab processes."),
                new Role(Guid.NewGuid(), "Normal User", RoleKey.NORMAL_USER, "Patient who can view their test results."),
            };

            foreach (var role in rolesToSeed)
                await context.Roles.AddAsync(role);

            await context.SaveChangesAsync();

            // -----------------------------
            // 2️ Seed Privileges (keep comments)
            // -----------------------------
            var privilegeList = new (string Name, string Description)[]
            {
                // Test order service
                // Test Order Controller
                ("ViewTestOrder", "Have right to view test order."),
                ("CreateTestOrder", "Have right to create a new patient test order."),
                ("DeleteTestOrder", "Have right to delete an existing test order."),
                ("ReadOnly", "Only have right to view patient test orders and results."),
                // Test Parameter Controller
                // (Anonymous)
                // Comment + Test Result Controller
                ("ModifyTestOrder", "Have right to modify patient test order information."),
                // File Controller
                ("GetFile", "Have right to get file from cloud."),
                ("UploadFile", "Have right to upload file from cloud."),
                ("DeleteFile", "Have right to delete file from cloud."),
                // Audit Log Controller
                ("ReadAuditLog", "Have right to view audit logs in the system."),

                // Warehouse service (SERVICE, LAB_MANAGER, LAB_USER)
                // Configuration Controller
                ("ViewConfiguration", "Have right to view system configurations."),
                ("ModifyConfiguration", "Have right to modify configurations."),
                ("CreateConfiguration", "Have right to create a new configuration."),
                ("DeleteConfiguration", "Have right to delete configurations."),
                // Instrument Controller
                ("ViewInstrument", "Have right to view all instruments."),
                ("AddInstrument", "Have right to add new instruments."),
                ("CheckInstrumentStatus", "Have right to check instrument status."),
                ("ActivateDeactivateInstrument", "Have right to activate or deactivate instruments."),
                // Order Supply History Controller
                ("ViewOrderSupplyHistory", "Have right to view order instrument supplyment."),
                ("CreateOrderSupplyHistory", "Have right to create new order instrument supplyment."),
                ("ModifyOrderSupplyHistory", "Have right to update existed order instrument supplyment."),
                // Parameter Controller
                ("ViewParameter", "Have right to view existed instrument parameter."),
                ("ModifyParameter", "Have right to update existed instrument parameter."),
                // Reagent Controller
                ("ViewReagent", "Have right to view reagents."),
                // Reagent Usage History Controller
                ("ViewReagentUsageHistory", "Have right to view reagent usage history."),

                // IAM service
                // User Controller
                ("ViewUser", "Have right to view all user profiles."),
                ("CreateUser", "Have right to create new users."),
                ("ModifyUser", "Have right to modify user profiles."),
                ("DeleteUser", "Have right to delete users."),
                ("ChangePassword", "Have right to change owned account password"),
                // Role Controller
                ("ViewRole", "Have right to view all roles and privileges."),
                ("CreateRole", "Have right to create new custom roles."),
                ("UpdateRole", "Have right to update role privileges."),
                ("DeleteRole", "Have right to delete custom roles."),
                // Privilege Controller
                ("ViewPrivilege", "Have right to view all system privileges."),
                ("CreatePrivilege", "Have right to create new privileges."),
                ("UpdatePrivilege", "Have right to update existing privileges."),
                ("DeletePrivilege", "Have right to delete privileges."),

                // Instrument service
                ("CreateBarcode", "Have right to create new barcode."),
                ("UpdateInstrumentMode", "Have right to update instrument to ready or set QC confirmation."),


                // Patient service
                // Patient Controller + Medical Record Controller
                ("ViewPatient", "Have right to view patient."),
                ("CreatePatient", "Have right to create patient."),
                ("UpdatePatient", "Have right to update patient."),
                ("DeletePatient", "Have right to delete patient.")
            };

            foreach (var (name, desc) in privilegeList)
            {
                if (!await context.Privileges.AnyAsync(p => p.Name == name))
                    await context.Privileges.AddAsync(new Privilege(Guid.NewGuid(), name, desc));
            }

            await context.SaveChangesAsync();

            // -----------------------------
            // 3️ Seed Role-Privilege mappings (IAM only)
            // -----------------------------
            var rolesFromDb = await context.Roles.ToListAsync();
            var privilegesFromDb = await context.Privileges.ToListAsync();

            var rolePrivilegesToInsert = new List<RolePrivilege>();

            void AddPrivilegesToRole(string roleCode, params string[] privilegeNames)
            {
                var role = rolesFromDb.First(r => r.Code == roleCode);
                foreach (var name in privilegeNames)
                {
                    var privilege = privilegesFromDb.First(p => p.Name == name);
                    rolePrivilegesToInsert.Add(new RolePrivilege(Guid.NewGuid(), role.RoleID, privilege.PrivilegeID, true));
                }
            }

            // SUPER_ADMIN → all privileges
            AddPrivilegesToRole(RoleKey.SUPER_ADMIN, privilegesFromDb.Select(p => p.Name).ToArray());

            // ADMIN → all privileges except role/privilege management (limitation)
            var adminPrivileges = privilegesFromDb
                .Where(p => !new[]
                {
                    "CreateRole", "UpdateRole", "DeleteRole",
                    "CreatePrivilege", "UpdatePrivilege", "DeletePrivilege"
                }.Contains(p.Name))
                .Select(p => p.Name)
                .ToArray();
            AddPrivilegesToRole(RoleKey.ADMIN, adminPrivileges);

            // LAB_MANAGER
            AddPrivilegesToRole(RoleKey.LAB_MANAGER,
                "ViewUser", "CreateUser", "ModifyUser", "DeleteUser",
                "ViewRole", "ViewPrivilege", "ChangePassword",
                "ViewInstrument", "ActivateDeactivateInstrument",
                "ViewOrderSupplyHistory", "CreateOrderSupplyHistory", "ModifyOrderSupplyHistory",
                "ViewReagentUsageHistory"
            );

            // SERVICE
            AddPrivilegesToRole(RoleKey.SERVICE,
                "ChangePassword",
                "ViewConfiguration", "CreateConfiguration", "ModifyConfiguration", "DeleteConfiguration",
                "ViewInstrument", "AddInstrument", "CheckInstrumentStatus", "ActivateDeactivateInstrument",
                "ViewOrderSupplyHistory", "CreateOrderSupplyHistory", "ModifyOrderSupplyHistory",
                "ViewParameter", "ModifyParameter", "ViewReagent",
                "ViewReagentUsageHistory"
            );

            // LAB_USER
            AddPrivilegesToRole(RoleKey.LAB_USER,
                "ChangePassword",

                // Warehouse
                "ViewConfiguration", "CreateConfiguration", "ModifyConfiguration", "DeleteConfiguration",
                "ViewInstrument", "CheckInstrumentStatus", "ActivateDeactivateInstrument",
                "ViewOrderSupplyHistory", "CreateOrderSupplyHistory", "ModifyOrderSupplyHistory",
                "ViewReagentUsageHistory",

                // Patient
                "ViewPatient", "CreatePatient", "UpdatePatient", "DeletePatient",

                // Instrument
                "UpdateInstrumentMode", "CreateBarcode",

                // Test Order
                "ModifyTestOrder",
                "GetFile", "UploadFile", "DeleteFile",
                "ViewTestOrder", "CreateTestOrder", "DeleteTestOrder",
                "ReadOnly"
            );

            // NORMAL_USER
            AddPrivilegesToRole(RoleKey.NORMAL_USER,
                "ChangePassword",
                "ReadOnly",
                "ViewPatient",
                "ViewTestOrder"
            );

            await context.RolePrivileges.AddRangeAsync(rolePrivilegesToInsert);
            await context.SaveChangesAsync();

            // -----------------------------
            // 4️ Seed Default Super Admin User (constructor)
            // -----------------------------
            var adminEmail = "longdong32120@gmail.com";
            if (!await context.Users.AnyAsync(u => u.Email == adminEmail))
            {
                var adminRole = rolesFromDb.First(r => r.Code == RoleKey.SUPER_ADMIN);

                var adminUser = new User(
                    userID: Guid.NewGuid(),
                    email: adminEmail,
                    fullName: "Dong Xuan Bao Long",
                    dob: new DateTime(2005, 1, 28),
                    address: "Hiep Phuoc, Nhon Trach, Dong Nai",
                    gender: "Male",
                    phone: "+84349331141",
                    password: "28012005",
                    identityNumber: "077205011495",
                    createdBy: "077205011495",
                    isActive: true
                );

                adminUser.AddRole(adminRole.RoleID);

                await context.Users.AddAsync(adminUser);
                await context.SaveChangesAsync();
            }
        }
    }
}
