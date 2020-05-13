import React from 'react';
import logo from './logo.svg';
import './App.css';

function App() {
  //SkillData();
  return (
    <div className="App">
      <header className="App-header" >
        <p>
          Template Code
          <br></br>
          <input id="template" type="text" onChange={handleChange}/>
          <br></br>
          <button onClick={handleClick} id="template-process">Generate</button>
        </p>

        <div id="skills">
          <div id="skill-1" class="skill tooltip">
            <span class="tooltiptext">

              <div id="tooltiptext-icons">
                <img class="tooltiptext-img" src="https://wiki.guildwars.com/images/b/b9/Vow_of_Strength.jpg" alt=""></img>
                <br></br>
                <img class="tooltiptext-energy" src="https://wiki.guildwars.com/images/b/be/Tango-energy.png" alt=""></img>5
                <br></br>
                <img class="tooltiptext-activation" src="https://wiki.guildwars.com/images/a/aa/Tango-activation-darker.png" alt=""></img>1/4
                <br></br>
                <img class="tooltiptext-activation" src="https://wiki.guildwars.com/images/f/f4/Tango-recharge-darker.png" alt=""></img>20
              </div>
              <div id="tooltiptext-text">
                <h1><div id="tooltip-skill-name">
                  Vow of Strength
              </div></h1>
                <h2>
                  <div id="tooltip-skill-type">
                    Elite Enchantment
                </div>
                </h2>
                <h3>
                  <div id="tooltip-skill-description">
                    For 15 seconds, whenever you attack a foe with your scythe, you deal 10...22...25 slashing damage to all adjacent foes.
                </div>
                </h3>
              </div>
            </span>
            <img class="skill-img" src="https://wiki.guildwars.com/images/b/b9/Vow_of_Strength.jpg" alt=""></img>
          </div>
          <div id="skill-2" class="skill tooltip">
            <span class="tooltiptext">Tooltip text</span>
            <img class="skill-img" src="https://wiki.guildwars.com/images/b/b9/Vow_of_Strength.jpg" alt=""></img>
          </div>
          <div id="skill-3" class="skill tooltip">
            <span class="tooltiptext">Tooltip text</span>
            <img class="skill-img" src="https://wiki.guildwars.com/images/b/b9/Vow_of_Strength.jpg" alt=""></img>
          </div>
          <div id="skill-4" class="skill tooltip">
            <span class="tooltiptext">Tooltip text</span>
            <img class="skill-img" src="https://wiki.guildwars.com/images/b/b9/Vow_of_Strength.jpg" alt=""></img>
          </div>
          <div id="skill-5" class="skill tooltip">
            <span class="tooltiptext">Tooltip text</span>
            <img class="skill-img" src="https://wiki.guildwars.com/images/b/b9/Vow_of_Strength.jpg" alt=""></img>
          </div>
          <div id="skill-6" class="skill tooltip">
            <span class="tooltiptext">Tooltip text</span>
            <img class="skill-img" src="https://wiki.guildwars.com/images/b/b9/Vow_of_Strength.jpg" alt=""></img>
          </div>
          <div id="skill-7" class="skill tooltip">
            <span class="tooltiptext">Tooltip text</span>
            <img class="skill-img" src="https://wiki.guildwars.com/images/b/b9/Vow_of_Strength.jpg" alt=""></img>
          </div>
          <div id="skill-8" class="skill tooltip">
            <span class="tooltiptext">Tooltip text</span>
            <img class="skill-img" src="https://wiki.guildwars.com/images/b/b9/Vow_of_Strength.jpg" alt=""></img>
          </div>
        </div>

      </header>
    </div>
  );
}

var templateCodeInput = "";

function handleChange(e){
  templateCodeInput = e.target.value;
  console.log(templateCodeInput);
}

function handleClick(){
  SkillData(templateCodeInput);
}

function SkillData(code) {
  

  var templateString = code;
  console.log(templateString);
  var binary;

  console.log(templateString.length);

  for(var i =0; i < templateString.length; i++){
    var numericValue = _Base64ToValue[templateString.charCodeAt(i)];
    if(numericValue == -1){
      console.log("Incorrect value in the template string at position " + i);
      return false;
    }
    binary += WriteBits(numericValue);
  }

  console.log(binary);

  var completeBinary = "";
  for(var i = 0; i < binary.length; i++){
    for(var j = 0; j < binary[i].length; j++){
      if(binary[i][j] == 0 || binary[i][j] == 1){
        completeBinary += binary[i][j];
      }
    }
    
  }

  console.log(completeBinary);

  var binaryIndex = 0;

  var templateType = parseInt(completeBinary.substr(binaryIndex,4).split("").reverse().join(""),2);
  binaryIndex += 4;
  var versionNumber = completeBinary.substr(binaryIndex,4).split("").reverse().join("");
  binaryIndex += 4;
  var professionController = (parseInt(completeBinary.substr(8,2).split("").reverse().join(""),2) * 2) + 4;
  binaryIndex += 2;  
  var mainProfession = parseInt(completeBinary.substr(binaryIndex, professionController).split("").reverse().join(""),2);
  console.log("Main profession: " + professionTable[mainProfession]);
  binaryIndex += professionController;
  var secondaryProfession = parseInt(completeBinary.substr(binaryIndex,professionController).split("").reverse().join(""),2);
  console.log("Secondary profession: " + professionTable[secondaryProfession]);
  binaryIndex += professionController;
  var attributeCount = parseInt(completeBinary.substr(binaryIndex,4).split("").reverse().join(""),2);
  console.log("Attribute count: " + attributeCount);
  binaryIndex += 4;
  var attributeController = parseInt(completeBinary.substr(binaryIndex, 4).split("").reverse().join(""),2) + 4;
  binaryIndex += 4;

  var attributes = {};
  for(var a = 0; a < attributeCount; a++){
    var attributeId = parseInt(completeBinary.substr(binaryIndex,attributeController).split("").reverse().join(""),2);
    binaryIndex += attributeController;
    var attributePoints = parseInt(completeBinary.substr(binaryIndex,4).split("").reverse().join(""),2);
    binaryIndex += 4;
    console.log(attributeTable[attributeId] + " " + attributePoints);
    attributes += [attributeId,attributePoints];
  }


  var skillController = parseInt(completeBinary.substr(binaryIndex,4).split("").reverse().join(""),2) + 8;
  binaryIndex += 4;

  var skills = {};

  for(var a = 0; a < 8; a++){
      var skill =  parseInt(completeBinary.substr(binaryIndex,skillController).split("").reverse().join(""),2);
      console.log(skill);
      skills += skill;
      var result = skillTable.find(obj => {
        return obj.Id == skill;
      });
      console.log(result);
      if(result != undefined){
        console.log(result["Name"]);
      }
      binaryIndex += skillController;
  }




  
}

function WriteBits(val){
var buff = [
  ((val >> 0) & 1),
  ((val >> 1) & 1),
  ((val >> 2) & 1),
  ((val >> 3) & 1),
  ((val >> 4) & 1),
  ((val >> 5) & 1)
]
return buff;
}

export default App;

var skillTable = [
  {
    "Id": 1759,
    "Name": "Vow of Strength",
    "Type": "Elite Enchantment",
    "Attribute": "Earth Prayers",
    "Description": "For 15 seconds, whenever you attack a foe with your scythe, you deal {0} slashing damage to all adjacent foes.",
    "Energy Cost": "5",
    "Recharge Rate": "20",
    "Activation": "1/4",
    "Upkeep": null,
    "Adrenaline": null,
    "Sacrifice": null,
    "Overcast": null,
    "Ranks": { "0": [10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30] },
    "Image": "https://wiki.guildwars.com/images/b/b9/Vow_of_Strength.jpg"
  },
  {
    "Id": 1762,
    "Name": "Heart of Fury",
    "Type": "Stance",
    "Attribute": "Mysticism",
    "Description": "For {0} seconds, you attack 25% faster.",
    "Energy Cost": null,
    "Recharge Rate": null,
    "Activation": null,
    "Upkeep": null,
    "Adrenaline": "4",
    "Sacrifice": null,
    "Overcast": null,
    "Ranks": { "0": [2, 3, 3, 4, 4, 5, 5, 6, 6, 7, 7, 8, 8, 9, 9, 10, 11, 11, 12, 12, 13] },
    "Image": "https://wiki.guildwars.com/images/0/03/Heart_of_Fury.jpg"
  }

]

var _Base64ToValue = [
  -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, // [0,   16)
  -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, // [16,  32)
  -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 62, -1, -1, -1, 63, // [32,  48)
  52, 53, 54, 55, 56, 57, 58, 59, 60, 61, -1, -1, -1, -1, -1, -1, // [48,  64)
  -1, 0,  1,  2,  3,  4,  5,  6,  7,  8,  9,  10, 11, 12, 13, 14, // [64,  80)
  15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, -1, -1, -1, -1, -1, // [80,  96)
  -1, 26, 27, 28, 29, 30, 31, 32, 33, 34, 35, 36, 37, 38, 39, 40, // [96,  112)
  41, 42, 43, 44, 45, 46, 47, 48, 49, 50, 51, -1, -1, -1, -1, -1, // [112, 128)
];
var professionTable = [
  "Any",
  "Warrior",
  "Ranger",
  "Monk",
  "Necromancer",
  "Mesmer",
  "Elementalist",
  "Assassin",
  "Ritualist",
  "Paragon",
  "Dervish"
]
var attributeTable = [
  "Fast Casting",
  "Illusion Magic",
  "Domination Magic",
  "Inspiriation Magic",
  "Blood Magic",
  "Death Magic",
  "Soul Reaping",
  "Curses",
  "Air Magic",
  "Earth Magic",
  "Fire Magic",
  "Water Magic",
  "Energy Storage",
  "Healing Prayers",
  "Smiting Prayers",
  "Protection Prayers",
  "Divine Favor",
  "Strength",
  "Axe Mastery",
  "Hammer Mastery",
  "Swordsmanship",
  "Tactics",
  "Beast Mastery",
  "Expertise",
  "Wilderness Survival",
  "Marksmanship",
  null,
  null,
  null,
  "Dagger Mastery",
  "Deadly Arts",
  "Shadow Arts",
  "Communing",
  "Resortation Magic",
  "Channeling Magic",
  "Critical Strikes",
  "Spawning Power",
  "Spear Mastery",
  "Command",
  "Motiviation",
  "Leadership",
  "Scythe Mastery",
  "Wind Prayers",
  "Earth Prayers",
  "Mysticism"
]