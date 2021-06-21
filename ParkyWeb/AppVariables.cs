namespace ParkyWeb
{
    public static class AppVariables
    {
        //API url
        public static string APIBaseUrl = "https://localhost:44346/";

        //park route
        public static string NationalParkAPIPath = APIBaseUrl+ "api/v1/nationalparks/";
        
        //trail route
        public static string TrailAPIPath = APIBaseUrl+ "api/v1/trails/";
        //User route
        public static string AccountAPIPath = APIBaseUrl + "api/v1/Users/";


        //sesssion name of Token
        public static string JWTokenSession = "JWToken";

        //Data temo for alert message
        public static string TempDataAlert = "alert";



    }
}
