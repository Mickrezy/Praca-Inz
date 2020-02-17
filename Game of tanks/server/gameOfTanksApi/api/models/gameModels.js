'use strict';
var mongoose = require('mongoose');
var Schema = mongoose.Schema;

var GameSchema = new Schema({
  players: [String],
  map: Number,
  ended: Boolean,
  draw: Boolean,
  winner: String,
  looser: String
},
{
  versionKey: false
});

var TurretSchema = new Schema({
  turretQuat1: Number,
  turretQuatI: Number,
  turretQuatJ: Number,
  turretQuatK: Number,
  turretAngle: Number,
  turretRotationTime: Number
},
{
  versionKey: false
});

var BulletSchema = new Schema({
  isAlive: Boolean,
  id: Number,
  coX: Number,
  coY: Number,
  quat1: Number,
  quatI: Number,
  quatJ: Number,
  quatK: Number,
  speed: Number
},
{
  versionKey: false
});

var TankSchema = new Schema({
  isAlive: Boolean,
  coX: Number,
  coY: Number,
  quat1: Number,
  quatI: Number,
  quatJ: Number,
  quatK: Number,
  turret: TurretSchema,
  forceLeft: Number,
  forceRight: Number
},
{
  versionKey: false
});

var GameDataSchema = new Schema({
  game: String,
  player : String,
  timer: Number,
  objects: [TankSchema],
  bullets: [BulletSchema],
  baseHealth: Number
},
{
  versionKey: false
});

var PlayerSchema = new Schema({
  name: {type: String, unique: true},
  wins: Number,
  defeats: Number,
  draws: Number
},
{
  versionKey: false
});


module.exports = mongoose.model('Bullet', BulletSchema);
module.exports = mongoose.model('Turret', TurretSchema);
module.exports = mongoose.model('Game', GameSchema);
module.exports = mongoose.model('GameData', GameDataSchema);
module.exports = mongoose.model('Tank', TankSchema);
module.exports = mongoose.model('Player', PlayerSchema);
