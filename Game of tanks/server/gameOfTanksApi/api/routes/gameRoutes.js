'use strict';
module.exports = function(app) {
  var gameOfTanks = require('../controllers/gameController');


  app.route('/stats/:playerName')
    .get(gameOfTanks.show_my_stats);	   //zapytanie zwracające statyski gracza o podanej nazwie

  app.route('/game')
	  .get(gameOfTanks.create_new_game);	/*Zapytanie sprawdzające czy jest jakaś gra w kolejce, jeżeli nie, to
										                    tworzy nową grę, losuje mapę i dodaje ją do kolejki, jeżeli jest już
										                    utworzona gra w kolejce, to ściąga ją z kolejki i rozsyła graczom*/

  app.route('/game/:gameID')
    .get(gameOfTanks.give_me_oponent)			      /*Zapytanie wysyłane w sytuacji gdy gracz ma już utworzoną gre
												                        i czeka na połączenie z przeciwnikiem. Zwraca ono grę z wpisanymi graczami */

    .put(gameOfTanks.update_after_game_stats);	/*Zapytanie aktualizujące statystki obu graczy po rozgrywce*/

  app.route('/cancel/:gameID')
    .get(gameOfTanks.cancel_my_game);         //Zapytanie anulujące szukanie gracza, zwracające potwierdzenie anulowania

  app.route('/game/:gameID/:playerID')
    .get(gameOfTanks.read_player_game_stats)  /*Zapytanie zwracające dane przeciwnika podczas rozgrywki*/

    .post(gameOfTanks.send_my_game_stats)     /*Zapytanie wysyłające dane gracza do bazy danych, by przeciwnik mógł je odczytać*/

    .put(gameOfTanks.update_my_game_stats);   /*Zapytanie aktualizujące dane gracza w bazie danych*/

  app.route('/register')
    .post(gameOfTanks.register);		        /*Zapytanie rejestrujące gracza w bazie danych
        										                Zapytanie zwraca potwierdzenie rejestracji lub ewentualne błędy*/
  app.route('/login')
    .post(gameOfTanks.login);			          /*Zapytanie służace do logowania się w grze.
										                        Zapytanie zwraca potwierdzenie zalogowania lub ewentualne błędy*/
  app.route('/logout')
    .get(gameOfTanks.logout);			        /*Zapytanie służace do wylogowywania się z gry
										                      Zapytanie zwraca potwierdzenie zalogowania lub ewentualne błędy*/

};
