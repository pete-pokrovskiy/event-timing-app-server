//using Croc.CFB.DAL;
//using Croc.CFB.Domain;
//using Croc.CFB.Logic.Services;
//using Croc.CFB.Mailing;
//using Microsoft.Extensions.Configuration;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace Croc.CFB.Logic
//{
//    public class DelayedMailProcessingService : IDelayedMailProcessingService
//    {
//        private readonly ICfbUow _uow;
//        private readonly IConfiguration _config;
//        private readonly IMailingService _mailingService;
//        private readonly IDelayedMailProcessingParams _delayedMailProcessingParams;

//        public DelayedMailProcessingService(ICfbUow uow,
//                                            IConfiguration config,
//                                            IMailingService mailingService,
//                                            IDelayedMailProcessingParams delayedMailProcessingParams)
//        {
//            _uow = uow;
//            _config = config;
//            _mailingService = mailingService;
//            _delayedMailProcessingParams = delayedMailProcessingParams;
//        }

//        /// <summary>
//        /// Отправляем письма из хранилища
//        /// </summary>
//        public void SendMailFromDataStore()
//        {
//            List<Email> mailsToSend = new List<Email>();

//            //забираем все письма из БД со статусом "К отправке"
//            var readyToBeSentEmails = _uow.EmailRepository.FindBy(e => e.DeliveryStatus == Domain.Enums.EmailDeliveryStatus.ReadyToBeSent && (e.EmailType == null || e.EmailType != null && !e.EmailType.Disabled));

//            mailsToSend.AddRange(readyToBeSentEmails);

//            if (_delayedMailProcessingParams.SendEmailsInErrorState)
//            {
//                var errorStateEmails = _uow.EmailRepository.FindBy(e => e.DeliveryStatus == Domain.Enums.EmailDeliveryStatus.Error);
//                mailsToSend.AddRange(errorStateEmails);
//            }

//            if (!mailsToSend.Any())
//                return;

//            decimal numberOfSendIterations = Math.Round((decimal)(mailsToSend.Count / _delayedMailProcessingParams.SendBatchSize), MidpointRounding.AwayFromZero);

//            for (int i = 0; i <= numberOfSendIterations; i++)
//            {
//                var mailBatchToSend = mailsToSend.Skip(i * _delayedMailProcessingParams.SendBatchSize).Take(_delayedMailProcessingParams.SendBatchSize);
                   
//                foreach(var mail in mailBatchToSend)
//                {
//                    try
//                    {
//                        _mailingService.SendMailWithLogo(new MailingData
//                        {
//                            Body = mail.Body,
//                            Subject = mail.Subject,
//                            ToRecipients = mail.ToRecipients,
//                            CcRecipients = mail.CcRecipients,
//                            BccRecipients = mail.BccRecipients,
//                            From = mail.From,
//                            MailPriority = mail.MailPriority
//                        });

//                        mail.SentDate = DateTime.Now;
//                        mail.DeliveryStatus = Domain.Enums.EmailDeliveryStatus.Sent;

//                    }
//                    catch (Exception ex)
//                    {
//                        //TODO: оповещение об ошибке + логирование
//                        mail.DeliveryStatus = Domain.Enums.EmailDeliveryStatus.Error;
//                        mail.ErrorMessage = ex.ToString();
//                    }

//                    _uow.EmailRepository.Update(mail);
//                }
//                //Parallel.ForEach(mailBatchToSend, m =>
//                //{

//                //});

//                //коммитим по батчам
//                _uow.Commit();
                
//                //не нужно делать задержку, если уже все отправили
//                Task.Delay(_delayedMailProcessingParams.SendIterationsTimeout).Wait();
//            }


//        }
//    }
//}
