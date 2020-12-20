﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Syndication;
using System.ServiceModel.Web;
using System.Text;

namespace PWS_7
{
    // ПРИМЕЧАНИЕ. Команду "Переименовать" в меню "Рефакторинг" можно использовать для одновременного изменения имени интерфейса "IFeed1" в коде и файле конфигурации.
    [ServiceContract]
    [ServiceKnownType(typeof(Atom10FeedFormatter))]
    [ServiceKnownType(typeof(Rss20FeedFormatter))]
    [ServiceKnownType(typeof(List<Note>))]
    public interface IFeed1
    {
        [OperationContract]
        [WebGet(UriTemplate = "students/{studentId}/notes", BodyStyle = WebMessageBodyStyle.Bare)]
        SyndicationFeedFormatter GetStudentNotes(string studentId);

        // TODO: Добавьте здесь операции служб
    }
}
