namespace Nagih
{
    public static class Error
    {
        // scope error nya, setiap scope dibagi menjadi tipe eror
        public enum Scope
        {
            Success = 0,
            Expected = 1,
            Connection = 2,
            AuthServer = 3,
            Unexpected = 9
        }
        
        // jenis error ada apa aja
        public enum Type
        {
            NoError = 0,

            Undefined = 1001,
            WrongBodyData = 1002,
            RequestTypeNotFound = 1003,
            DatabaseError = 1004,
            WrongHeaderData = 1005,
            JSON = 1901,

            UserNotExist = 2001,
            UserAlreadyExist = 2002,

            TransactionIdNotValid = 2501,
            InsufficientCoin = 2502,
            AchievementAlreadyRedeem = 2503,
            AchievementUnfinish = 2504,
            CheatDetected = 2505,
            ItemAlreadyPurchased = 2506,

            AuthError = 3001,
            TokenExpired = 3002,

            NoConnection = 9001,
            RequestNotValid = 9002,

            Unexpected = 9999
        }

        public static Scope GetScope(int type)
        {
            return GetScope((Type)type);
        }

        public static Scope GetScope(Type type)
        {
            switch (type)
            {
                case Type.NoError:
                    return Scope.Success;

                case Type.UserNotExist:
                case Type.UserAlreadyExist:
                case Type.TokenExpired:
                case Type.InsufficientCoin:
                case Type.AchievementAlreadyRedeem:
                case Type.AchievementUnfinish:
                case Type.CheatDetected:
                case Type.ItemAlreadyPurchased:
                    return Scope.Expected;

                case Type.AuthError:
                    return Scope.AuthServer;

                case Type.NoConnection:
                case Type.RequestNotValid:
                    return Scope.Connection;

                default:
                    return Scope.Unexpected;
            }
        }
    }
}
