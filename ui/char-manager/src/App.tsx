import { useSelector } from 'react-redux'
import CharacterInfo from './components/CharacterInfo'
import { selectIsConnected } from './slices/connectionSlice'
import ActionBar from './components/ActionBar/ActionBar';
import { api } from './api';
import { charactersSelectors } from './slices/characterSlice';
import { DamageTypes } from './types/damageTypes';
import classes from './App.module.css';
import CharacterSelector from './components/CharacterSelector';
import { useState } from 'react';
import { blankCharacter } from './types/character';

export default function App() {
  const connection = useSelector(selectIsConnected);
  const characters = useSelector(charactersSelectors.selectAll);

  const [selectedCharacterName, setSelectedCharacterName] = useState<string>(blankCharacter.name);

  const selectedCharacter = characters.find((c) => c.name === (selectedCharacterName !== "" ? selectedCharacterName : characters[0].name)) ?? blankCharacter;

  return (
    <>
      <header className={classes.header}>
        <h1>Character Manager</h1>
      </header>
      <main className={classes.main}>
        {connection ?
          (
            <>
              <CharacterSelector characterNames={characters.map(c => c.name)} onCharacterSelect={(c) => setSelectedCharacterName(c)} />
              <CharacterInfo character={selectedCharacter} />
              <ActionBar
                onDamageClick={(amount: number, damageType: DamageTypes) => api.dealDamage(selectedCharacter.name, { amount: amount, incomingDamageType: damageType })}
                onHealClick={(amount: number) => api.healDamage(selectedCharacter.name, { amount: amount })}
                onAddTempHpClick={(amount: number) => api.AddTempHp(selectedCharacter.name, { amount: amount })}
              />
            </>
          ) :
          <> Not Connected </>
        }
      </main>
      <footer></footer>
    </>
  )
}
