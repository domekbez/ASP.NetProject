
using static CVEditor.Startup;

namespace CVEditor
{
    public static class UserTypeManager
    {
        public static readonly IService StaffData = ServiceLocator.Current.GetInstance<IService>();
    }
}