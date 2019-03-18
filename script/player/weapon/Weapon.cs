/*************************************************************************/
/*  Weapon.cs                                                            */
/*************************************************************************/
/*                       This file is part of:                           */
/*                           DungeonPath                                 */
/*             https://github.com/inthelamp/dungeonpath                  */
/*************************************************************************/
/* Copyright (c) 2018-2019 Dong Won Kim.                                 */
/*                                                                       */
/* Permission is hereby granted, free of charge, to any person obtaining */
/* a copy of this software and associated documentation files (the       */
/* "Software"), to deal in the Software without restriction, including   */
/* without limitation the rights to use, copy, modify, merge, publish,   */
/* distribute, sublicense, and/or sell copies of the Software, and to    */
/* permit persons to whom the Software is furnished to do so, subject to */
/* the following conditions:                                             */
/*                                                                       */
/* The above copyright notice and this permission notice shall be        */
/* included in all copies or substantial portions of the Software.       */
/*                                                                       */
/* THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,       */
/* EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF    */
/* MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.*/
/* IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY  */
/* CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT,  */
/* TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE     */
/* SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.                */
/*************************************************************************/
using Godot;
using System;

public class AttackAsset
{
  public int Speed { get; set; }	//Speed of attack
  public string Name { get; set; }	//Name of attack
  public string FileName { get; set; }   //File name of attack
  public bool IsEnabled { get; set; }    //Is it ready to use?
}

public abstract class Weapon : Node
{
  public string PlayerPath { get; set; } //node path of the one who possesses this weapon.
  public int Level { get; set; } //level
  public int MaximumAP { get; set; } //Maximum attack points
  public int MinimumAP { get; set; } //Minimum attack points
  public System.Collections.Generic.Dictionary<string, AttackAsset> AttackAssets { get; set; }
  = new System.Collections.Generic.Dictionary<string, AttackAsset>();

  //Set MaximumAP
  public abstract void SetMaximumAP(int level);

  //Set MinimumAP
  public abstract void SetMinimumAP(int level);

  //Random number between MinimumAP and MaximumAP
  public abstract int GetAttackPoints();
}
