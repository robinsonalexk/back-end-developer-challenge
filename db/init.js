const fs = require('fs');

const mydb = db.getSiblingDB('charmanagerdb');
const files = [
  '/docker-entrypoint-initdb.d/briv.json',
  '/docker-entrypoint-initdb.d/ryu.json'
];

for (const file of files) {
  const character = JSON.parse(fs.readFileSync(file, 'utf8'));
  character._id = character.name;
  character.currentHitPoints = character.hitPoints;
  delete character.name;

  mydb.characters.insertOne(character);
  print(`Inserted ${character._id} from ${file}`);
}