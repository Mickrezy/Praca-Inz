
  var fs = require('fs');
  var http = require('http');
  var https = require('https');
  var privateKey  = fs.readFileSync('sslcert/server.key', 'utf8');
  var certificate = fs.readFileSync('sslcert/server.crt', 'utf8');
  var credentials = {key: privateKey, cert: certificate};

  var express = require('express');
  //var port = process.env.PORT;
  //var portSecure = process.env.PORT;
  var port = 8080;
  var portSecure = 8081;
  var routes = require('./api/routes/gameRoutes'); //importing route
  var mongoose = require('mongoose');
  var cookieParser = require('cookie-parser');
  var session = require('express-session');

  //var dataModels = require('./api/models/gameModels'); //created model loading here
  var bodyParser = require('body-parser');

  // mongoose instance connection url connection
  mongoose.Promise = global.Promise;
  mongoose.connect('mongodb://localhost/Tankdb');

 //logging into the file
  var fs = require('fs');
  var util = require('util');
  var log_file = fs.createWriteStream(__dirname + '/logs.log', {flags : 'w'});
  var log_stdout = process.stdout;

  console.log = function(d) {
    log_file.write(util.format(d) + '\n');
    log_stdout.write(util.format(d) + '\n');
  };


  var app = express();

  app.use(bodyParser.urlencoded({ extended: true }));
  app.use(bodyParser.json());                     // get information from html forms
  app.use(cookieParser());                        // read cookies (needed for auth)
  app.set('view engine', 'ejs');

  //use sessions for tracking logins
  app.use(session({
    secret: 'pemko to zrobil',
    resave: true,
    httpOnly: true,
    secure: true,
    saveUninitialized: false
  }));

  var httpServer = http.createServer(app);
  var httpsServer = https.createServer(credentials, app);

  routes(app);      //register the route
  httpServer.listen(port);
  httpsServer.listen(portSecure);


console.log('game of tanks RESTful API server started on: ' + port);
