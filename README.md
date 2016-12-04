# Unity-ThirdPersonController

Third Person Controller for Unity. </br>
I don't use physics and <a href="http://docs.unity3d.com/ScriptReference/Rigidbody.html">Rigidbody</a> to control the character. Instead I use a <a href="http://docs.unity3d.com/Manual/class-CharacterController.html">CharacterController</a> component and custom gravity.

![gif](https://github.com/dbrizov/dbrizov.github.io/blob/master/images/project-images/Character%20Controller/camera_occlusion.gif)
![gif](https://github.com/dbrizov/dbrizov.github.io/blob/master/images/project-images/Character%20Controller/jogging.gif)
![gif](https://github.com/dbrizov/dbrizov.github.io/blob/master/images/project-images/Character%20Controller/jumping.gif)
![gif](https://github.com/dbrizov/dbrizov.github.io/blob/master/images/project-images/Character%20Controller/sprinting.gif)
![gif](https://github.com/dbrizov/dbrizov.github.io/blob/master/images/project-images/Character%20Controller/walking.gif)

## System Requirements

Unity 5.0 or later versions.<br />
I actually created the controller with Unity 5.0 and haven't tried to run it with an older version. If you are using an older version of Unity, feel free to try it out. I don't think there are any reasons for it not to run in Unity 4.6.

## Used Assets

The character and the animations are taken from <a href="https://www.mixamo.com/">Mixamo</a>. Just check it out. All of the characters are free to use. There are also many free animations, and the first 20 animations you buy are free of charge.

## Controls

**Keyboard And Mouse**
- Movement: **WASD**
- Camera: **Mouse**
- Jump: **Space**
- Toggle Walk/Run: **Caps Lock**
- Sprint: **Left Shift**
- Toggle Slow Motion: **Tab**

**XBOX 360/ONE Controller**
- Movement: **Left Stick**
- Camera: **Right Stick**
- Jump: **A**
- Sprint: **LB**
- Toggle Slow Motion: **RB**

## License

The MIT License (MIT)

Copyright (c) 2015 Denis Rizov

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
