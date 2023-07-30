# WhistleSharp

WhistleSharp is a music sheet generation GUI tool for Tin Whistle, based on LilyPond.

## Notation

WhistleSharp uses a custom notation for Tin Whistle. The notation is based on fingering and whistle key.

#### Example Finger notation and their respective notes on a D Whistle:

| Fingering Notation | Key of D Note |
|--------------------|---------------|
| 6                  | d             |
| 5                  | e             |
| 4                  | f♯            |
| 3                  | g             |
| 2                  | a             |
| 1                  | b             |
| 0                  | c♯            |
| 7                  | c             |
| 8                  | D             |

`7` represents the C natural note, while `8` represents the D note special fingering one octave above.

### Higher Octave

Overblowing is represented by adding a `+` after the fingering notation. `8+` represents the D note two octaves above.

### Half Holing and Chromatic Notes

Half holing is represented by adding a `,` after the fingering notation.

### Note Duration

Note duration is represented by adding `q` after the fingering notation. `'` after the `q` will shorten the note by a quarter for each `'` added.

| Notation | Note          |
|----------|---------------|
| q'       | Quarter       |
| q''      | Eighth        |
| q'''     | Sixteenth     |
| q''''    | Thirty-second |
| q'''''   | Sixty-fourth  |

`.` can be added to make the note dotted.
## Installation

Currently only Windows is supported. You can download the latest release [here](https://github.com/Saismirk/WhistleSharp/releases).

## Usage

### Note Input

While in the Input tab, you can input notes at the top text box. Adding or removing notes will update the bottom sheet preview.

### File Settings

While in the Settings tab, you can modify the following sheet settings:

#### Title

The title of the sheet.

#### Composer

The composer of the sheet.

#### Copyright

The copyright shown at the bottom of the sheet.

#### Filename

The filename of the sheet. The file will be saved as a `.ly` file and also generate a `.png` of the sheet.


#### Output Directory

The directory where the sheet will be saved.

### Sheet Settings

#### Key

The key of the sheet. This will change the key of the sheet and the note that each fingering represents.

#### Tempo

The tempo of the sheet. This also affects the tempo of the generate MIDI file.

#### Time Signature

The time signature of the sheet.

## Export

You may click the `Generate` button to generate the sheet. The sheet will be saved as a `.ly` file and also generate a `.png` of the sheet.

### MIDI

You can click the `Play MIDI` button to play the generated MIDI file using a Tin Whistle SoundFont.

## Saving and Loading

You can save and load your sheet as a `.json` file. This will save all the settings and notes you have inputted.

## License

WhistleSharp is licensed under the MIT license. See [LICENSE](LICENSE) for more information.

