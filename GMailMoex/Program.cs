using Google.Apis.Auth.OAuth2;
using Google.Apis.Gmail.v1;
using Google.Apis.Gmail.v1.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GMailMoex
{
    class Program
    {
        // If modifying these scopes, delete your previously saved credentials
        // at ~/.credentials/gmail-dotnet-quickstart.json
        static string[] Scopes = { GmailService.Scope.GmailReadonly };
        static string ApplicationName = "Gmail API .NET Quickstart";



        static void Main(string[] args)
        {
            UserCredential credential;

            using (var stream =
                new FileStream("credentials.json", FileMode.Open, FileAccess.Read))
            {
                // The file token.json stores the user's access and refresh tokens, and is created
                // automatically when the authorization flow completes for the first time.
                string credPath = "token.json";
                credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(stream).Secrets,
                    Scopes,
                    "user",
                    CancellationToken.None,
                    new FileDataStore(credPath, true)).Result;
                Console.WriteLine("Credential file saved to: " + credPath);
            }

            // Create Gmail API service.
            var service = new GmailService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName,
            });

            // Get list messages
            var messages = ListMessages(service, "me", "from: mailer@moex.com");
            if (messages != null && messages.Count > 0)
            {
                foreach (var item in messages)
                {

                    // Save attachments to TEMP
                    GetAttachments(service, "me", item.Id,Environment.GetEnvironmentVariable("TEMP"));
                }
            }
        }




        /// <summary>
        /// List all Messages of the user's mailbox matching the query.
        /// </summary>
        /// <param name="service">Gmail API service instance.</param>
        /// <param name="userId">User's email address. The special value "me"
        /// can be used to indicate the authenticated user.</param>
        /// <param name="query">String used to filter Messages returned.</param>
        static List<Message> ListMessages(GmailService service, String userId, String query)
        {
            List<Message> result = new List<Message>();
            UsersResource.MessagesResource.ListRequest request = service.Users.Messages.List(userId);
            request.Q = query;


            do
            {
                try
                {
                    ListMessagesResponse response = request.Execute();
                    
                    string listfile = Path.GetFileNameWithoutExtension(System.Reflection.Assembly.GetExecutingAssembly().Location) + ".list";
                    string[] SavedId = {"0"};

                    if (File.Exists(listfile))
                    {
                        SavedId = File.ReadAllLines(listfile);
                    }

                    // исключаем сообщения, выгруженные ранее
                    var messages = response.Messages.Where(m => !SavedId.Contains(m.Id));

                    if (messages != null && messages.ToList().Count > 0)
                        result.AddRange(messages);

                    request.PageToken = response.NextPageToken;
                }
                catch (Exception e)
                {
                    string error = e.Message;
                    if (e.InnerException != null) error = e.InnerException.Message;

                    string logfile = Path.GetFileNameWithoutExtension(System.Reflection.Assembly.GetExecutingAssembly().Location) + ".log";
                    WriteErrorToLog(logfile, e.ToString());

                    Console.WriteLine(e.Message);
                }
            } while (!String.IsNullOrEmpty(request.PageToken));

            return result;
        }

        // пишем в лог-файл
        static void WriteErrorToLog(string logfile, string errormessage)
        {

            File.AppendAllText(logfile, Environment.NewLine + DateTime.Now + Environment.NewLine + errormessage, System.Text.Encoding.GetEncoding(1251));
        }


        /// <summary>
        /// Get and store attachment from Message with given ID.
        /// </summary>
        /// <param name="service">Gmail API service instance.</param>
        /// <param name="userId">User's email address. The special value "me"
        /// can be used to indicate the authenticated user.</param>
        /// <param name="messageId">ID of Message containing attachment.</param>
        /// <param name="outputDir">Directory used to store attachments.</param>
        static void GetAttachments(GmailService service, String userId, String messageId, String outputDir)
        {
            try
            {
                Message message = service.Users.Messages.Get(userId, messageId).Execute();
                var parts = message.Payload.Parts;

                foreach (MessagePart part in parts)
                {
                    if (!String.IsNullOrEmpty(part.Filename))
                    {
                        String attId = part.Body.AttachmentId;
                        MessagePartBody attachPart = service.Users.Messages.Attachments.Get(userId, messageId, attId).Execute();

                        // Converting from RFC 4648 base64 to base64url encoding
                        // see http://en.wikipedia.org/wiki/Base64#Implementations_and_history
                        String attachData = attachPart.Data.Replace('-', '+');
                        attachData = attachData.Replace('_', '/');

                        byte[] data = Convert.FromBase64String(attachData);
                        File.WriteAllBytes(Path.Combine(outputDir, part.Filename), data);

                    }
                }

                // сохраним ID сообщения, чтобы не выгружать его в последующем
                string listfile = Path.GetFileNameWithoutExtension(System.Reflection.Assembly.GetExecutingAssembly().Location) + ".list";
                if (!File.Exists(listfile))
                    File.AppendAllText(listfile, messageId);
                else
                    File.AppendAllText(listfile, Environment.NewLine+messageId);
            }
            catch (Exception e)
            {
                string error = e.Message;
                if (e.InnerException != null) error = e.InnerException.Message;

                string logfile = Path.GetFileNameWithoutExtension(System.Reflection.Assembly.GetExecutingAssembly().Location) + ".log";
                WriteErrorToLog(logfile, e.ToString());

                Console.WriteLine(e.Message);
            }
        }

    }
}
