namespace Boozfinder.Helpers
{
    public class Enums
    {
        public enum HashType
        {
            SHA256,
            SHA384,
            SHA512
        }

        public enum Rating
        {
            ThumbDown,
            ThumbNeutral,
            ThumbUp
        }

        public enum Role
        {
            User,
            Admin
        }

        public enum Task
        {
            Create,
            Update
        }

        public enum Type
        {
            Booze,
            User,
            Review
        }
    }
}
