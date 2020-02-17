'use strict';

  var mongoose = require('mongoose');

  var userModel = require('../models/users');
  var User = mongoose.model('User');
  var models = require('../models/gameModels');
  var Game = mongoose.model('Game');
  var GameData = mongoose.model('GameData');
  var Player = mongoose.model('Player');
  var Bullet = mongoose.model('Bullet');
  var Queue = require('queue-fifo');  //Queue-fifo
  var queue = new Queue();
  var Random = require('random-int');



/////////////////////////////////////////////     A E S T H E T I C
///
///            SHOW MY STATS
///
////////////////////////////////////////////
exports.show_my_stats = function(req, res){
  if(req.session)
  {
    Player.findOne({'name': req.params.playerName}, function(err, reqStats){
      if(err)
        res.status(400).send(err);
      else{
        res.json(reqStats);
      }
    });
  }
};


/////////////////////////////////////////////
///
///          UPDATE MY STATS
///
////////////////////////////////////////////
exports.update_after_game_stats = function(req, res){
  var gameID = req.params.gameID;
  Game.findOneAndUpdate({'_id': gameID}, {new: true}, {
    draw: req.body.draw,
    winner: req.body.winner,
    looser: req.body.looser
  },function(err,reqGame){
    if (err)
      res.send(err);
    else if(reqGame.ended == false)
      {
        if(req.body.draw == false)
        {
          Player.findOne({'name': req.body.winner}, function(err,player){
            if(err)
              res.send(err);
            else
            {
              player.wins = player.wins + 1;
              player.save(function (err) { if (err)  return res.send(err);});
            }
          });
          Player.findOne({'name': req.body.looser}, function(err,player){
            if(err)
              res.send(err);
            else
            {
              player.defeats = player.defeats + 1;
              player.save(function (err) { if (err) return res.send(err);});
            }
          });
        }
        else
        {
          Player.findOne({'name': reqGame.players[0]},function(err,player){
            if(err)
              res.send(err);
            else
            {
              player.draws = player.draws + 1;
              player.save(function (err) { if (err) return res.send(err);});
            }
          });
          Player.findOne({'name': reqGame.players[1]},function(err,player){
            if(err)
              res.send(err);
            else
            {
              player.draws = player.draws + 1;
              player.save(function (err) { if (err) return res.send(err);});
            }
          });
        }
        reqGame.ended = true;
        reqGame.save(function (err) {if (err) return res.send(err);});
        res.status(204).send();
      }
    else{
      res.status(204).send();
    }
  });
};



/////////////////////////////////////////////
///
///            REGISTRATION
///
////////////////////////////////////////////
exports.register = function(req, res){
  if (req.body.email && req.body.username && req.body.password && req.body.passwordConf)
  {
    Player.findOne({'name': req.body.username} , function(err, player){
        if(err)
          res.send(err);
        else if(player != null)
          return res.status(401).send('Już istnieje gracz o takiej nazwie');
        else
        {
          User.findOne({'email': req.body.email}, function(err,user){
            if(err)
              res.send(err);
            else if(user != null)
              return res.status(402).send('Już istnieje użytkownik o takim emailu');
            else{
              var userData = {
                email: req.body.email,
                username: req.body.username,
                password: req.body.password,
                passwordConf: req.body.passwordConf
              };
              var playerData = {
                name: req.body.username,
                wins: 0,
                defeats: 0,
                draws: 0
              };
              Player.create(playerData, function(err){
                if(err)
                 return res.send(err);
              });
              User.create(userData, function (err, user) {
                if (err) {
                  res.send(err);
                } else {
                  return res.status(200).send('Zarejestrowano :)');
                }
              });
            }
          });
        }
    });
  }
};


/////////////////////////////////////////////
///
///                LOGIN
///
////////////////////////////////////////////
exports.login = function(req, res){
  if (req.body.login && req.body.logpassword) {
    User.authenticate(req.body.login, req.body.logpassword, function (error, user) {
      if (error || !user) {
        return res.status(401).send('Złe hasło lub login spróbuj jeszcze raz :)');  // 401 - Unauthorized
      } else {
        req.session.userId = user._id;
        return res.status(200).send('Zalogowano :)');       // 200 - ok
      }
    });
  } else {
    return res.status(400).send('Wymagane wszystkie pola');  // 400 - bad request
  }
};


/////////////////////////////////////////////
///
///                 LOGOUT
///
////////////////////////////////////////////
exports.logout = function (req, res, next) {
  if (req.session) {
    // delete session object
    req.session.destroy(function (err) {
      if (err) {
        return next(err);
      } else {
         res.status(200).send('Wylogowano :)');
      }
    });
  }
};


/////////////////////////////////////////////
///
///           CREATING NEW GAME
///
////////////////////////////////////////////
exports.create_new_game = function(req, res){
  var player = req.query.playerID;
  if(queue.isEmpty())
  {
    var map = Random(0,1);
    var newGame = new Game({players: [player], ended: false, map: map});
    newGame.save(function (err,game) {
        queue.enqueue(game);
        if(err)
          return res.send(err);
        res.json(game);
    });
  }
  else{
    var first_game_in_queue = queue.peek();
    if(first_game_in_queue.players[0] == player)
    {
      queue.dequeue();
      res.status(500).send('Chcesz grac sam ze soba');
    }
    else
    {
      first_game_in_queue.players.push(player);
      first_game_in_queue.save();
      queue.dequeue();
      res.json(first_game_in_queue);
    }
  }
};


/////////////////////////////////////////////
///
///           JOINING TO THE GAME
///
////////////////////////////////////////////
exports.give_me_oponent = function(req, res){
  Game.findOne({'_id': req.params.gameID}, function(err, reqGame) {
    if (err)
      res.send(err);
    else{
      res.json(reqGame);
    }
  });
};


/////////////////////////////////////////////
///
///         CANCELING THE GAME
///
////////////////////////////////////////////
exports.cancel_my_game = function(req,res){
  Game.remove({'_id': req.params.gameID}, function(err) {
    if (err)
      res.send(err);
    else{
      console.log("Canceled looking for game and deleted");
      res.status(200).send('OK');
    }
  });
};

/////////////////////////////////////////////
///
///      READING SOMEONES GAME DATA
///
////////////////////////////////////////////
exports.read_player_game_stats = function(req, res){
  GameData.findOne({'game': req.params.gameID ,'player': req.params.playerID }, function(err, gameData) {
      if (err)
        res.send(err);
      else{
        //console.log("czyjes "gameData.timer);
        res.json(gameData);
        }
    });
};


/////////////////////////////////////////////
///
///           SENDING MY GAME DATA
///
////////////////////////////////////////////
exports.send_my_game_stats = function(req, res){
  var player = req.params.playerID;
  Game.findOne({'_id': req.params.gameID}, function(err, reqGame) {
    if (err)
      res.send(err);
    else if(reqGame.players.indexOf(player) > -1 )
    {
        var gameData = new GameData(req.body);
        gameData.save(function (err) {
            if (err){
              return res.send(err);
            }
          });
      res.json(gameData);
    }
  });
};


/////////////////////////////////////////////
///
///           UPDATING MY GAME DATA
///
////////////////////////////////////////////
exports.update_my_game_stats = function(req, res){

  GameData.findOneAndUpdate({'_id': req.body._id},
    {
      baseHealth: req.body.baseHealth,
      bullets: req.body.bullets,
      objects: req.body.objects,
      timer: req.body.timer,
  }, {new: true}, function(err, gameData) {
      if (err)
        res.send(err);
      else{
        res.json(gameData);
      }
    });
};
