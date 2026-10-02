import { render, screen, within } from '@testing-library/react';
import { describe, it, expect } from 'vitest';
import CharacterInfo from './CharacterInfo';
import type { Character } from '../../types/character';
import { DamageTypes } from '../../types/damageTypes';
import { DefenseTypes } from '../../types/defenseTypes';

const baseCharacter: Character = {
  name: 'Gerund',
  level: 5,
  currentHitPoints: 30,
  hitPoints: 42,
  tempHitPoints: 5,
  classes: [
    { name: 'cleric', classLevel: 3, hitDiceValue: 0 },
    { name: 'warrior', classLevel: 2, hitDiceValue: 0 },
  ],
  items: [],
  stats: {
    strength: 8,
    dexterity: 16,
    constitution: 14,
    intelligence: 18,
    wisdom: 12,
    charisma: 10,
  },
  defenses: [
    { type: DamageTypes.Fire, defense: DefenseTypes.Resistance },
    { type: DamageTypes.Poison, defense: DefenseTypes.Immunity },
  ],
};

describe('CharacterSheet', () => {
  describe('when no character is selected', () => {
    it('shows the empty state message', () => {
      render(<CharacterInfo character={undefined} />);
      expect(screen.getByText(/No Character Selected/i)).toBeInTheDocument();
    });

    it('does not render any section headings', () => {
      render(<CharacterInfo character={undefined} />);
      expect(screen.queryByRole('heading', { level: 4 })).not.toBeInTheDocument();
    });
  });

  describe('when a character is provided', () => {
    it('does not show the empty state message', () => {
      render(<CharacterInfo character={baseCharacter} />);
      expect(screen.queryByText(/No Character Selected/i)).not.toBeInTheDocument();
    });

    it('renders all four section headings', () => {
      render(<CharacterInfo character={baseCharacter} />);
      ['General', 'Class Info', 'Ability Scores', 'Defenses'].forEach((name) => {
        expect(screen.getByRole('heading', { name })).toBeInTheDocument();
      });
    });

    it('renders general info', () => {
      render(<CharacterInfo character={baseCharacter} />);
      expect(screen.getByText('Name: Gerund')).toBeInTheDocument();
      expect(screen.getByText('Level: 5')).toBeInTheDocument();
      expect(screen.getByText('Hit Points: 30/42')).toBeInTheDocument();
      expect(screen.getByText('Temporary Hit Points: 5')).toBeInTheDocument();
    });

    it('gives hit points an accessible label', () => {
      render(<CharacterInfo character={baseCharacter} />);
      expect(
        screen.getByLabelText('Hit Points: 30 out of 42')
      ).toBeInTheDocument();
    });

    it('renders each class with a capitalized name and level', () => {
      render(<CharacterInfo character={baseCharacter} />);
      expect(screen.getByText('Cleric: Level 3')).toBeInTheDocument();
      expect(screen.getByText('Warrior: Level 2')).toBeInTheDocument();
    });

    it('renders all six ability scores', () => {
      render(<CharacterInfo character={baseCharacter} />);
      const section = screen
        .getByRole('heading', { name: 'Ability Scores' })
        .closest('section')!;
      const items = within(section).getAllByRole('listitem');

      expect(items).toHaveLength(6);
      expect(items[0]).toHaveTextContent('Strength: 8');
      expect(items[1]).toHaveTextContent('Dexterity: 16');
      expect(items[2]).toHaveTextContent('Constitution: 14');
      expect(items[3]).toHaveTextContent('Intelligence: 18');
      expect(items[4]).toHaveTextContent('Wisdom: 12');
      expect(items[5]).toHaveTextContent('Charisma: 10');
    });

    it('renders each defense', () => {
      render(<CharacterInfo character={baseCharacter} />);
      expect(screen.getByText('Fire: Resistance')).toBeInTheDocument();
      expect(screen.getByText('Poison: Immunity')).toBeInTheDocument();
    });
  });
});