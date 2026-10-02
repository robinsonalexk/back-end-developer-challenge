import { useState } from "react";
import Card from "../Card";
import classes from "./CharacterSelector.module.css"

interface Props {
    characterNames: string[];
    onCharacterSelect: (characterName: string) => void;
}

export default function CharacterSelector(props: Props) {
    const { characterNames, onCharacterSelect } = props;

    const [selectedCharacter, setSelectedCharacter] = useState<string>("");

    function onSelectedCharacterChanged(character: string) {
        setSelectedCharacter(character)
        onCharacterSelect(character);
    }

    return (
        <Card>
            <div className={classes.characterSelectorSection}>
                <label>Select Character
                    <select value={selectedCharacter} onChange={(e) => onSelectedCharacterChanged(e.target.value)}>
                        {Object.values(characterNames).map((cn) => {
                            return (
                                <option key={cn} value={cn}>{cn}</option>
                            )
                        })}
                    </select>
                </label>
                <img src="/user.svg" alt="Image Representing Character" />
            </div>
        </Card>
    );
}