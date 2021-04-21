using System;
using System.Linq;
using LaylasLittleCompanion.Server.Models;
using TrelloNet;

namespace LaylasLittleCompanion.Server.Services
{
    public class TrelloService
    {
        private readonly TrelloSettings _trelloSettings;
        private readonly ITrello _trello;

        public TrelloService(TrelloSettings trelloSettings)
        {
            _trelloSettings = trelloSettings;

            _trello = new Trello(_trelloSettings.ApiKey);

            _trello.Authorize(_trelloSettings.Token);

        }


        public string AddNewCardAsync(NewTrelloCard card)
        {
            try
            {
                var list = _trelloSettings
               .TrelloLists.FirstOrDefault(l => l.Name.ToLower() == card.ListName.ToLower());
                var listActual = _trello.Lists.WithId(list.Id);
                var board = _trello
                    .Boards.WithId(_trelloSettings.BoardId);

                Card trelloCard = _trello
                    .Cards.Add(new NewCard(card.CardName, listActual));

                trelloCard.Desc = $"{card.UserName} suggests {card.Description}";

                _trello.Cards.Update(trelloCard);

                return "Your Trello card was added, thank you for your input!";
            }
            catch(Exception ex)
            {
                Console.WriteLine($"Error adding card: {ex}");
                return "There was an error adding your card :-(";
            }
        }
    }
}
