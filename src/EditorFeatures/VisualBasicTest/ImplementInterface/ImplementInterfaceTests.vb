' Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

Imports Microsoft.CodeAnalysis.CodeFixes
Imports Microsoft.CodeAnalysis.Diagnostics
Imports Microsoft.CodeAnalysis.Editor.VisualBasic.UnitTests.Diagnostics
Imports Microsoft.CodeAnalysis.VisualBasic.ImplementInterface

Namespace Microsoft.CodeAnalysis.Editor.VisualBasic.UnitTests.ImplementInterface
    Partial Public Class ImplementInterfaceTests
        Inherits AbstractVisualBasicDiagnosticProviderBasedUserDiagnosticTest

        Friend Overrides Function CreateDiagnosticProviderAndFixer(workspace As Workspace) As (DiagnosticAnalyzer, CodeFixProvider)
            Return (Nothing, New VisualBasicImplementInterfaceCodeFixProvider)
        End Function

        <WorkItem(540085, "http://vstfdevdiv:8080/DevDiv2/DevDiv/_workitems/edit/540085")>
        <Fact, Trait(Traits.Feature, Traits.Features.CodeActionsImplementInterface)>
        Public Async Function TestSimpleMethod() As Task
            Await TestAsync(
"Interface I
    Sub M()
End Interface
Class C
    Implements [|I|]
End Class",
"Imports System
Interface I
    Sub M()
End Interface
Class C
    Implements I
    Public Sub M() Implements I.M
        Throw New NotImplementedException()
    End Sub
End Class")
        End Function

        <WpfFact, Trait(Traits.Feature, Traits.Features.CodeActionsImplementInterface)>
        Public Async Function TestInterfaceWithTuple() As Task
            Await TestAsync(
"Imports System
Class Foo
    Implements [|IFoo|]
End Class
Interface IFoo
    Function Method(x As (Alice As Integer, Bob As Integer)) As (String, String)
End Interface",
"Imports System
Class Foo
    Implements IFoo
    Public Function Method(x As (Alice As Integer, Bob As Integer)) As (String, String) Implements IFoo.Method
        Throw New NotImplementedException()
    End Function
End Class
Interface IFoo
    Function Method(x As (Alice As Integer, Bob As Integer)) As (String, String)
End Interface")
        End Function

        <Fact, Trait(Traits.Feature, Traits.Features.CodeActionsImplementInterface)>
        Public Async Function TestMethodConflict1() As Task
            Await TestAsync(
"Interface I
    Sub M()
End Interface
Class C
    Implements [|I|]
    Function M() As Integer
    End Function
End Class",
"Imports System
Interface I
    Sub M()
End Interface
Class C
    Implements I
    Private Sub I_M() Implements I.M
        Throw New NotImplementedException()
    End Sub
    Function M() As Integer
    End Function
End Class")
        End Function

        <Fact, Trait(Traits.Feature, Traits.Features.CodeActionsImplementInterface)>
        Public Async Function TestMethodConflict2() As Task
            Await TestAsync(
"Interface IFoo
    Sub Bar()
End Interface
Class C
    Implements [|IFoo|]
    Public Sub Bar()
    End Sub
End Class",
"Imports System
Interface IFoo
    Sub Bar()
End Interface
Class C
    Implements IFoo
    Public Sub Bar()
    End Sub
    Private Sub IFoo_Bar() Implements IFoo.Bar
        Throw New NotImplementedException()
    End Sub
End Class")
        End Function

        <WorkItem(542012, "http://vstfdevdiv:8080/DevDiv2/DevDiv/_workitems/edit/542012")>
        <Fact, Trait(Traits.Feature, Traits.Features.CodeActionsImplementInterface)>
        Public Async Function TestMethodConflictWithField() As Task
            Await TestAsync(
"Interface I
    Sub M()
End Interface
Class C
    Implements [|I|]
    Private m As Integer
End Class",
"Imports System
Interface I
    Sub M()
End Interface
Class C
    Implements I
    Private m As Integer
    Private Sub I_M() Implements I.M
        Throw New NotImplementedException()
    End Sub
End Class")
        End Function

        <WorkItem(542015, "http://vstfdevdiv:8080/DevDiv2/DevDiv/_workitems/edit/542015")>
        <Fact, Trait(Traits.Feature, Traits.Features.CodeActionsImplementInterface)>
        Public Async Function TestAutoPropertyConflict() As Task
            Await TestAsync(
"Interface I
    Property M As Integer
End Interface
Class C
    Implements [|I|]
    Public Property M As Integer
End Class",
"Imports System
Interface I
    Property M As Integer
End Interface
Class C
    Implements I
    Public Property M As Integer
    Private Property I_M As Integer Implements I.M
        Get
            Throw New NotImplementedException()
        End Get
        Set(value As Integer)
            Throw New NotImplementedException()
        End Set
    End Property
End Class")
        End Function

        <WorkItem(542015, "http://vstfdevdiv:8080/DevDiv2/DevDiv/_workitems/edit/542015")>
        <Fact, Trait(Traits.Feature, Traits.Features.CodeActionsImplementInterface)>
        Public Async Function TestFullPropertyConflict() As Task
            Await TestAsync(
"Interface I
    Property M As Integer
End Interface
Class C
    Implements [|I|]
    Private Property M As Integer
        Get
            Return 5
        End Get
        Set(value As Integer)
        End Set
    End Property
End Class",
"Imports System
Interface I
    Property M As Integer
End Interface
Class C
    Implements I

    Private Property M As Integer
        Get
            Return 5
        End Get
        Set(value As Integer)
        End Set
    End Property

    Private Property I_M As Integer Implements I.M
        Get
            Throw New NotImplementedException()
        End Get
        Set(value As Integer)
            Throw New NotImplementedException()
        End Set
    End Property
End Class")
        End Function

        <WorkItem(542019, "http://vstfdevdiv:8080/DevDiv2/DevDiv/_workitems/edit/542019")>
        <Fact, Trait(Traits.Feature, Traits.Features.CodeActionsImplementInterface)>
        Public Async Function TestConflictFromBaseClass1() As Task
            Await TestAsync(
"Interface I
    Sub M()
End Interface
Class B
    Public Sub M()
    End Sub
End Class
Class C
    Inherits B
    Implements [|I|]
End Class",
"Imports System
Interface I
    Sub M()
End Interface
Class B
    Public Sub M()
    End Sub
End Class
Class C
    Inherits B
    Implements I
    Private Sub I_M() Implements I.M
        Throw New NotImplementedException()
    End Sub
End Class")
        End Function

        <WorkItem(542019, "http://vstfdevdiv:8080/DevDiv2/DevDiv/_workitems/edit/542019")>
        <Fact, Trait(Traits.Feature, Traits.Features.CodeActionsImplementInterface)>
        Public Async Function TestConflictFromBaseClass2() As Task
            Await TestAsync(
"Interface I
    Sub M()
End Interface
Class B
    Protected M As Integer
End Class
Class C
    Inherits B
    Implements [|I|]
End Class",
"Imports System
Interface I
    Sub M()
End Interface
Class B
    Protected M As Integer
End Class
Class C
    Inherits B
    Implements I
    Private Sub I_M() Implements I.M
        Throw New NotImplementedException()
    End Sub
End Class")
        End Function

        <WorkItem(542019, "http://vstfdevdiv:8080/DevDiv2/DevDiv/_workitems/edit/542019")>
        <Fact, Trait(Traits.Feature, Traits.Features.CodeActionsImplementInterface)>
        Public Async Function TestConflictFromBaseClass3() As Task
            Await TestAsync(
"Interface I
    Sub M()
End Interface
Class B
    Public Property M As Integer
End Class
Class C
    Inherits B
    Implements [|I|]
End Class",
"Imports System
Interface I
    Sub M()
End Interface
Class B
    Public Property M As Integer
End Class
Class C
    Inherits B
    Implements I
    Private Sub I_M() Implements I.M
        Throw New NotImplementedException()
    End Sub
End Class")
        End Function

        <Fact, Trait(Traits.Feature, Traits.Features.CodeActionsImplementInterface)>
        Public Async Function TestImplementAbstractly1() As Task
            Await TestAsync(
"Interface I
    Sub M()
End Interface
MustInherit Class C
    Implements [|I|]
End Class",
"Interface I
    Sub M()
End Interface
MustInherit Class C
    Implements I
    Public MustOverride Sub M() Implements I.M
End Class",
index:=1)
        End Function

        <Fact, Trait(Traits.Feature, Traits.Features.CodeActionsImplementInterface)>
        Public Async Function TestImplementGenericType() As Task
            Await TestAsync(
"Interface IInterface1(Of T)
    Sub Method1(t As T)
End Interface
Class [Class]
    Implements [|IInterface1(Of Integer)|]
End Class",
"Imports System
Interface IInterface1(Of T)
    Sub Method1(t As T)
End Interface
Class [Class]
    Implements IInterface1(Of Integer)
    Public Sub Method1(t As Integer) Implements IInterface1(Of Integer).Method1
        Throw New NotImplementedException()
    End Sub
End Class")
        End Function

        <Fact, Trait(Traits.Feature, Traits.Features.CodeActionsImplementInterface)>
        Public Async Function TestImplementGenericTypeWithGenericMethod() As Task
            Await TestAsync(
"Interface IInterface1(Of T)
    Sub Method1(Of U)(arg As T, arg1 As U)
End Interface
Class [Class]
    Implements [|IInterface1(Of Integer)|]
End Class",
"Imports System
Interface IInterface1(Of T)
    Sub Method1(Of U)(arg As T, arg1 As U)
End Interface
Class [Class]
    Implements IInterface1(Of Integer)
    Public Sub Method1(Of U)(arg As Integer, arg1 As U) Implements IInterface1(Of Integer).Method1
        Throw New NotImplementedException()
    End Sub
End Class")
        End Function

        <Fact, WorkItem(6623, "DevDiv_Projects/Roslyn"), Trait(Traits.Feature, Traits.Features.CodeActionsImplementInterface)>
        Public Async Function TestImplementGenericTypeWithGenericMethodWithNaturalConstraint() As Task
            Await TestAsync(
"Imports System.Collections.Generic
Interface IInterface1(Of T)
    Sub Method1(Of U As IList(Of T))(arg As T, arg1 As U)
End Interface
Class [Class]
    Implements [|IInterface1(Of Integer)|]
End Class",
"Imports System
Imports System.Collections.Generic
Interface IInterface1(Of T)
    Sub Method1(Of U As IList(Of T))(arg As T, arg1 As U)
End Interface
Class [Class]
    Implements IInterface1(Of Integer)
    Public Sub Method1(Of U As IList(Of Integer))(arg As Integer, arg1 As U) Implements IInterface1(Of Integer).Method1
        Throw New NotImplementedException()
    End Sub
End Class")
        End Function

        <Fact, WorkItem(6623, "DevDiv_Projects/Roslyn"), Trait(Traits.Feature, Traits.Features.CodeActionsImplementInterface)>
        Public Async Function TestImplementGenericTypeWithGenericMethodWithUnexpressibleConstraint() As Task
            Await TestAsync(
"Interface IInterface1(Of T)
    Sub Method1(Of U As T)(arg As T, arg1 As U)
End Interface
Class [Class]
    Implements [|IInterface1(Of Integer)|]
End Class",
"Imports System
Interface IInterface1(Of T)
    Sub Method1(Of U As T)(arg As T, arg1 As U)
End Interface
Class [Class]
    Implements IInterface1(Of Integer)
    Public Sub Method1(Of U As Integer)(arg As Integer, arg1 As U) Implements IInterface1(Of Integer).Method1
        Throw New NotImplementedException()
    End Sub
End Class")
        End Function

        <Fact, Trait(Traits.Feature, Traits.Features.CodeActionsImplementInterface)>
        Public Async Function TestImplementThroughFieldMember() As Task
            Await TestAsync(
"Interface I
    Sub M()
End Interface
Class C
    Implements [|I|]
    Private x As I
End Class",
"Imports System
Interface I
    Sub M()
End Interface
Class C
    Implements I
    Private x As I
    Public Sub M() Implements I.M
        Throw New NotImplementedException()
    End Sub
End Class")
        End Function

        <Fact, Trait(Traits.Feature, Traits.Features.CodeActionsImplementInterface)>
        Public Async Function TestImplementThroughFieldMember1() As Task
            Await TestAsync(
"Interface I
    Sub M()
End Interface
Class C
    Implements [|I|]
    Private x As I
End Class",
"Interface I
    Sub M()
End Interface
Class C
    Implements I
    Private x As I
    Public Sub M() Implements I.M
        x.M()
    End Sub
End Class",
index:=1)
        End Function

        <WorkItem(472, "https://github.com/dotnet/roslyn/issues/472")>
        <Fact, Trait(Traits.Feature, Traits.Features.CodeActionsImplementInterface)>
        Public Async Function TestImplementThroughFieldMemberRemoveUnnecessaryCast() As Task
            Await TestAsync(
"Imports System.Collections

NotInheritable Class X : Implements [|IComparer|]
    Private x As X
End Class",
"Imports System.Collections

NotInheritable Class X : Implements IComparer
    Private x As X

    Public Function Compare(x As Object, y As Object) As Integer Implements IComparer.Compare
        Return Me.x.Compare(x, y)
    End Function
End Class",
index:=1)
        End Function

        <WorkItem(472, "https://github.com/dotnet/roslyn/issues/472")>
        <Fact, Trait(Traits.Feature, Traits.Features.CodeActionsImplementInterface)>
        Public Async Function TestImplementThroughFieldMemberRemoveUnnecessaryCastAndMe() As Task
            Await TestAsync(
"Imports System.Collections

NotInheritable Class X : Implements [|IComparer|]
    Private a As X
End Class",
"Imports System.Collections

NotInheritable Class X : Implements IComparer
    Private a As X

    Public Function Compare(x As Object, y As Object) As Integer Implements IComparer.Compare
        Return a.Compare(x, y)
    End Function
End Class",
index:=1)
        End Function

        <Fact, Trait(Traits.Feature, Traits.Features.CodeActionsImplementInterface)>
        Public Async Function TestImplementThroughFieldMemberInterfaceWithNonStandardProperties() As Task
            Dim source =
<File>
Interface IFoo
    Property Blah(x As Integer) As Integer
    Default Property Blah1(x As Integer) As Integer
End Interface

Class C
    Implements [|IFoo|]
    Dim i1 As IFoo
End Class
</File>
            Dim expected =
<File>
Interface IFoo
    Property Blah(x As Integer) As Integer
    Default Property Blah1(x As Integer) As Integer
End Interface

Class C
    Implements IFoo
    Dim i1 As IFoo

    Public Property Blah(x As Integer) As Integer Implements IFoo.Blah
        Get
            Return i1.Blah(x)
        End Get
        Set(value As Integer)
            i1.Blah(x) = value
        End Set
    End Property

    Default Public Property Blah1(x As Integer) As Integer Implements IFoo.Blah1
        Get
            Return i1(x)
        End Get
        Set(value As Integer)
            i1(x) = value
        End Set
    End Property
End Class
</File>
            Await TestAsync(source, expected, index:=1)
        End Function



        <WorkItem(540355, "http://vstfdevdiv:8080/DevDiv2/DevDiv/_workitems/edit/540355")>
        <Fact, Trait(Traits.Feature, Traits.Features.CodeActionsImplementInterface)>
        Public Async Function TestMissingOnImplementationWithDifferentName() As Task
            Await TestMissingAsync(
"Interface I1(Of T)
    Function Foo() As Double
End Interface
Class M
    Implements [|I1(Of Double)|]
    Public Function I_Foo() As Double Implements I1(Of Double).Foo
        Return 2
    End Function
End Class")
        End Function

        <WorkItem(540366, "http://vstfdevdiv:8080/DevDiv2/DevDiv/_workitems/edit/540366")>
        <Fact, Trait(Traits.Feature, Traits.Features.CodeActionsImplementInterface)>
        Public Async Function TestWithMissingEndBlock() As Task
            Await TestAsync(
"Imports System
Class M
    Implements [|IServiceProvider|]",
"Imports System
Class M
    Implements IServiceProvider
    Public Function GetService(serviceType As Type) As Object Implements IServiceProvider.GetService
        Throw New NotImplementedException()
    End Function
End Class")
        End Function

        <WorkItem(540367, "http://vstfdevdiv:8080/DevDiv2/DevDiv/_workitems/edit/540367")>
        <Fact, Trait(Traits.Feature, Traits.Features.CodeActionsImplementInterface)>
        Public Async Function TestSimpleProperty() As Task
            Await TestAsync(
"Interface I1
    Property Foo() As Integer
End Interface
Class M
    Implements [|I1|]
End Class",
"Imports System
Interface I1
    Property Foo() As Integer
End Interface
Class M
    Implements I1
    Public Property Foo As Integer Implements I1.Foo
        Get
            Throw New NotImplementedException()
        End Get
        Set(value As Integer)
            Throw New NotImplementedException()
        End Set
    End Property
End Class")
        End Function

        <Fact, Trait(Traits.Feature, Traits.Features.CodeActionsImplementInterface)>
        Public Async Function TestArrayType() As Task
            Await TestAsync(
"Interface I
    Function M() As String()
End Interface
Class C
    Implements [|I|]
End Class",
"Imports System
Interface I
    Function M() As String()
End Interface
Class C
    Implements I
    Public Function M() As String() Implements I.M
        Throw New NotImplementedException()
    End Function
End Class")
        End Function

        <Fact, Trait(Traits.Feature, Traits.Features.CodeActionsImplementInterface)>
        Public Async Function TestImplementInterfaceWithByRefParameters() As Task
            Await TestAsync(
"Class C
    Implements [|I|]
    Private foo As I
End Class
Interface I
    Sub Method1(ByRef x As Integer, ByRef y As Integer, z As Integer)
    Function Method2() As Integer
End Interface",
"Imports System
Class C
    Implements I
    Private foo As I
    Public Sub Method1(ByRef x As Integer, ByRef y As Integer, z As Integer) Implements I.Method1
        Throw New NotImplementedException()
    End Sub
    Public Function Method2() As Integer Implements I.Method2
        Throw New NotImplementedException()
    End Function
End Class
Interface I
    Sub Method1(ByRef x As Integer, ByRef y As Integer, z As Integer)
    Function Method2() As Integer
End Interface")
        End Function

        <Fact, Trait(Traits.Feature, Traits.Features.CodeActionsImplementInterface)>
        Public Async Function TestImplementInterfaceWithTypeCharacter() As Task
            Await TestAsync(
"Interface I1
    Function Method1$()
End Interface
Class C
    Implements [|I1|]
End Class",
"Imports System
Interface I1
    Function Method1$()
End Interface
Class C
    Implements I1
    Public Function Method1() As String Implements I1.Method1
        Throw New NotImplementedException()
    End Function
End Class")
        End Function

        <Fact, Trait(Traits.Feature, Traits.Features.CodeActionsImplementInterface)>
        Public Async Function TestImplementInterfaceWithParametersTypeSpecifiedAsTypeCharacter() As Task
            Await TestAsync(
"Interface I1
    Sub Method1(ByRef arg#)
End Interface
Class C
    Implements [|I1|]
End Class",
"Imports System
Interface I1
    Sub Method1(ByRef arg#)
End Interface
Class C
    Implements I1
    Public Sub Method1(ByRef arg As Double) Implements I1.Method1
        Throw New NotImplementedException()
    End Sub
End Class")
        End Function

        <WorkItem(540403, "http://vstfdevdiv:8080/DevDiv2/DevDiv/_workitems/edit/540403")>
        <Fact, Trait(Traits.Feature, Traits.Features.CodeActionsImplementInterface)>
        Public Async Function TestMissingOnInterfaceWithJustADelegate() As Task
            Await TestMissingAsync(
"Interface I1
    Delegate Sub Del()
End Interface
Class C
    Implements [|I1|]
End Class")
        End Function

        <WorkItem(540381, "http://vstfdevdiv:8080/DevDiv2/DevDiv/_workitems/edit/540381")>
        <Fact, Trait(Traits.Feature, Traits.Features.CodeActionsImplementInterface)>
        Public Async Function TestOrdering1() As Task
            Await TestAsync(
"Class C
    Implements [|I|]
    Private foo As I
End Class
Interface I
    Sub Method1(ByRef x As Integer, ByRef y As Integer, z As Integer)
    Function Method2() As Integer
End Interface",
"Imports System
Class C
    Implements I
    Private foo As I
    Public Sub Method1(ByRef x As Integer, ByRef y As Integer, z As Integer) Implements I.Method1
        Throw New NotImplementedException()
    End Sub
    Public Function Method2() As Integer Implements I.Method2
        Throw New NotImplementedException()
    End Function
End Class
Interface I
    Sub Method1(ByRef x As Integer, ByRef y As Integer, z As Integer)
    Function Method2() As Integer
End Interface")
        End Function

        <WorkItem(540415, "http://vstfdevdiv:8080/DevDiv2/DevDiv/_workitems/edit/540415")>
        <Fact, Trait(Traits.Feature, Traits.Features.CodeActionsImplementInterface)>
        Public Async Function TestDefaultProperty1() As Task
            Await TestAsync(
"Interface I1
    Default Property Foo(ByVal arg As Integer)
End Interface
Class C
    Implements [|I1|]
End Class",
"Imports System
Interface I1
    Default Property Foo(ByVal arg As Integer)
End Interface
Class C
    Implements I1
    Default Public Property Foo(arg As Integer) As Object Implements I1.Foo
        Get
            Throw New NotImplementedException()
        End Get
        Set(value As Object)
            Throw New NotImplementedException()
        End Set
    End Property
End Class")
        End Function

        <Fact, Trait(Traits.Feature, Traits.Features.CodeActionsImplementInterface)>
        Public Async Function TestImplementNestedInterface() As Task
            Await TestAsync(
"Interface I1
    Sub Foo()
    Delegate Sub Del(ByVal arg As Integer)
    Interface I2
        Sub Foo(ByVal arg As Del)
    End Interface
End Interface
Class C
    Implements [|I1.I2|]
End Class",
"Imports System
Interface I1
    Sub Foo()
    Delegate Sub Del(ByVal arg As Integer)
    Interface I2
        Sub Foo(ByVal arg As Del)
    End Interface
End Interface
Class C
    Implements I1.I2
    Public Sub Foo(arg As I1.Del) Implements I1.I2.Foo
        Throw New NotImplementedException()
    End Sub
End Class")
        End Function

        <WorkItem(540402, "http://vstfdevdiv:8080/DevDiv2/DevDiv/_workitems/edit/540402")>
        <Fact, Trait(Traits.Feature, Traits.Features.CodeActionsImplementInterface)>
        Public Async Function TestArrayRankSpecifiers() As Task
            Await TestAsync(
"Interface I1
    Sub Method1(ByVal arg() As Integer)
End Interface
Class C
    Implements [|I1|]
End Class",
"Imports System
Interface I1
    Sub Method1(ByVal arg() As Integer)
End Interface
Class C
    Implements I1
    Public Sub Method1(arg() As Integer) Implements I1.Method1
        Throw New NotImplementedException()
    End Sub
End Class")
        End Function

        <WorkItem(540398, "http://vstfdevdiv:8080/DevDiv2/DevDiv/_workitems/edit/540398")>
        <Fact, Trait(Traits.Feature, Traits.Features.CodeActionsImplementInterface)>
        Public Async Function TestSimplifyImplementsClause() As Task
            Await TestAsync(
"Namespace ConsoleApplication
    Interface I1
        Sub Method1()
    End Interface
    Class C
        Implements [|I1|]
    End Class
End Namespace",
"Imports System
Namespace ConsoleApplication
    Interface I1
        Sub Method1()
    End Interface
    Class C
        Implements I1
        Public Sub Method1() Implements I1.Method1
            Throw New NotImplementedException()
        End Sub
    End Class
End Namespace",
parseOptions:=Nothing) ' Namespaces not supported in script
        End Function

        <WorkItem(541078, "http://vstfdevdiv:8080/DevDiv2/DevDiv/_workitems/edit/541078")>
        <Fact, Trait(Traits.Feature, Traits.Features.CodeActionsImplementInterface)>
        Public Async Function TestParamArray() As Task
            Await TestAsync(
"Interface I2
    Function G(ParamArray args As Double()) As Integer
End Interface
Class A
    Implements [|I2|]
End Class",
"Imports System
Interface I2
    Function G(ParamArray args As Double()) As Integer
End Interface
Class A
    Implements I2
    Public Function G(ParamArray args() As Double) As Integer Implements I2.G
        Throw New NotImplementedException()
    End Function
End Class")
        End Function

        <WorkItem(541092, "http://vstfdevdiv:8080/DevDiv2/DevDiv/_workitems/edit/541092")>
        <Fact, Trait(Traits.Feature, Traits.Features.CodeActionsImplementInterface)>
        Public Async Function TestShowForNonImplementedPrivateInterfaceMethod() As Task
            Await TestAsync(
"Interface I1
    Private Sub Foo()
End Interface
Class A
    Implements [|I1|]
End Class",
"Imports System
Interface I1
    Private Sub Foo()
End Interface
Class A
    Implements I1
    Public Sub Foo() Implements I1.Foo
        Throw New NotImplementedException()
    End Sub
End Class")
        End Function

        <WorkItem(541092, "http://vstfdevdiv:8080/DevDiv2/DevDiv/_workitems/edit/541092")>
        <Fact, Trait(Traits.Feature, Traits.Features.CodeActionsImplementInterface)>
        Public Async Function TestDoNotShowForImplementedPrivateInterfaceMethod() As Task
            Await TestMissingAsync(
"Interface I1
    Private Sub Foo()
End Interface
Class A
    Implements [|I1|]
    Public Sub Foo() Implements I1.Foo
        Throw New NotImplementedException()
    End Sub
End Class")
        End Function

        <WorkItem(542010, "http://vstfdevdiv:8080/DevDiv2/DevDiv/_workitems/edit/542010")>
        <Fact, Trait(Traits.Feature, Traits.Features.CodeActionsImplementInterface)>
        Public Async Function TestNoImplementThroughSynthesizedFields() As Task
            Await TestActionCountAsync(
"Interface I
    Sub M()
End Interface
Class C
    Implements [|I|]
    Public Property X As I
End Class",
count:=2)

            Await TestAsync(
"Interface I
    Sub M()
End Interface
Class C
    Implements [|I|]
    Public Property X As I
End Class",
"Imports System
Interface I
    Sub M()
End Interface
Class C
    Implements I
    Public Property X As I
    Public Sub M() Implements I.M
        Throw New NotImplementedException()
    End Sub
End Class")

            Await TestAsync(
"Interface I
    Sub M()
End Interface
Class C
    Implements [|I|]
    Public Property X As I
End Class",
"Interface I
    Sub M()
End Interface
Class C
    Implements I
    Public Property X As I
    Public Sub M() Implements I.M
        X.M()
    End Sub
End Class",
index:=1)
        End Function

        <WorkItem(768799, "http://vstfdevdiv:8080/DevDiv2/DevDiv/_workitems/edit/768799")>
        <Fact, Trait(Traits.Feature, Traits.Features.CodeActionsImplementInterface)>
        Public Async Function TestImplementIReadOnlyListThroughField() As Task
            Await TestAsync(
"Imports System.Collections.Generic
Class A
    Implements [|IReadOnlyList(Of Integer)|]
    Private field As Integer()
End Class",
"Imports System.Collections
Imports System.Collections.Generic
Class A
    Implements IReadOnlyList(Of Integer)
    Private field As Integer()

    Default Public ReadOnly Property Item(index As Integer) As Integer Implements IReadOnlyList(Of Integer).Item
        Get
            Return DirectCast(field, IReadOnlyList(Of Integer))(index)
        End Get
    End Property

    Public ReadOnly Property Count As Integer Implements IReadOnlyCollection(Of Integer).Count
        Get
            Return DirectCast(field, IReadOnlyList(Of Integer)).Count
        End Get
    End Property

    Public Function GetEnumerator() As IEnumerator(Of Integer) Implements IEnumerable(Of Integer).GetEnumerator
        Return DirectCast(field, IReadOnlyList(Of Integer)).GetEnumerator()
    End Function

    Private Function IEnumerable_GetEnumerator() As IEnumerator Implements IEnumerable.GetEnumerator
        Return DirectCast(field, IReadOnlyList(Of Integer)).GetEnumerator()
    End Function
End Class",
index:=1)
        End Function

        <WorkItem(768799, "http://vstfdevdiv:8080/DevDiv2/DevDiv/_workitems/edit/768799")>
        <Fact, Trait(Traits.Feature, Traits.Features.CodeActionsImplementInterface)>
        Public Async Function TestImplementIReadOnlyListThroughProperty() As Task
            Await TestAsync(
"Imports System.Collections.Generic
Class A
    Implements [|IReadOnlyList(Of Integer)|]
    Private Property field As Integer()
End Class",
"Imports System.Collections
Imports System.Collections.Generic
Class A
    Implements IReadOnlyList(Of Integer)

    Default Public ReadOnly Property Item(index As Integer) As Integer Implements IReadOnlyList(Of Integer).Item
        Get
            Return DirectCast(field, IReadOnlyList(Of Integer))(index)
        End Get
    End Property

    Public ReadOnly Property Count As Integer Implements IReadOnlyCollection(Of Integer).Count
        Get
            Return DirectCast(field, IReadOnlyList(Of Integer)).Count
        End Get
    End Property

    Private Property field As Integer()

    Public Function GetEnumerator() As IEnumerator(Of Integer) Implements IEnumerable(Of Integer).GetEnumerator
        Return DirectCast(field, IReadOnlyList(Of Integer)).GetEnumerator()
    End Function

    Private Function IEnumerable_GetEnumerator() As IEnumerator Implements IEnumerable.GetEnumerator
        Return DirectCast(field, IReadOnlyList(Of Integer)).GetEnumerator()
    End Function
End Class",
index:=1)
        End Function

        <WorkItem(768799, "http://vstfdevdiv:8080/DevDiv2/DevDiv/_workitems/edit/768799")>
        <Fact, Trait(Traits.Feature, Traits.Features.CodeActionsImplementInterface)>
        Public Async Function TestImplementInterfaceThroughField() As Task
            Await TestAsync(
"Interface I
    Sub M()
End Interface
Class A
    Implements I
    Public Sub M() Implements I.M
    End Sub
End Class

Class B
    Implements [|I|]
    Dim x As A
End Class",
"Interface I
    Sub M()
End Interface
Class A
    Implements I
    Public Sub M() Implements I.M
    End Sub
End Class

Class B
    Implements I
    Dim x As A
    Public Sub M() Implements I.M
        DirectCast(x, I).M()
    End Sub
End Class",
index:=1)
        End Function

        <WorkItem(768799, "http://vstfdevdiv:8080/DevDiv2/DevDiv/_workitems/edit/768799")>
        <Fact, Trait(Traits.Feature, Traits.Features.CodeActionsImplementInterface)>
        Public Async Function TestImplementInterfaceThroughField_FieldImplementsMultipleInterfaces() As Task
            Await TestActionCountAsync(
"Interface I
    Sub M()
End Interface
Interface I2
    Sub M2()
End Interface
Class A
    Implements I, I2
    Public Sub M() Implements I.M, I2.M2
    End Sub
End Class

Class B
    Implements [|I|]
    Implements I2
    Dim x As A
End Class",
count:=2)

            Await TestActionCountAsync(
"Interface I
    Sub M()
End Interface
Interface I2
    Sub M2()
End Interface
Class A
    Implements I, I2
    Public Sub M() Implements I.M, I2.M2
    End Sub
End Class

Class B
    Implements I
    Implements [|I2|]
    Dim x As A
End Class",
count:=2)

            Await TestAsync(
"Interface I
    Sub M()
End Interface
Interface I2
    Sub M2()
End Interface
Class A
    Implements I, I2
    Public Sub M() Implements I.M, I2.M2
    End Sub
End Class

Class B
    Implements [|I|]
    Implements I2
    Dim x As A
End Class",
"Interface I
    Sub M()
End Interface
Interface I2
    Sub M2()
End Interface
Class A
    Implements I, I2
    Public Sub M() Implements I.M, I2.M2
    End Sub
End Class

Class B
    Implements I
    Implements I2
    Dim x As A
    Public Sub M() Implements I.M
        DirectCast(x, I).M()
    End Sub
End Class",
index:=1)

            Await TestAsync(
"Interface I
    Sub M()
End Interface
Interface I2
    Sub M2()
End Interface
Class A
    Implements I, I2
    Public Sub M() Implements I.M, I2.M2
    End Sub
End Class

Class B
    Implements I
    Implements [|I2|]
    Dim x As A
End Class",
"Interface I
    Sub M()
End Interface
Interface I2
    Sub M2()
End Interface
Class A
    Implements I, I2
    Public Sub M() Implements I.M, I2.M2
    End Sub
End Class

Class B
    Implements I
    Implements I2
    Dim x As A
    Public Sub M2() Implements I2.M2
        DirectCast(x, I2).M2()
    End Sub
End Class",
index:=1)
        End Function


        <WorkItem(768799, "http://vstfdevdiv:8080/DevDiv2/DevDiv/_workitems/edit/768799")>
        <Fact, Trait(Traits.Feature, Traits.Features.CodeActionsImplementInterface)>
        Public Async Function TestImplementInterfaceThroughField_MultipleFieldsCanImplementInterface() As Task
            Await TestActionCountAsync(
"Interface I
    Sub M()
End Interface
Class A
    Implements I
    Public Sub M() Implements I.M
    End Sub
End Class

Class B
    Implements [|I|]
    Dim x As A
    Dim y As A
End Class",
count:=3)

            Await TestAsync(
"Interface I
    Sub M()
End Interface
Class A
    Implements I
    Public Sub M() Implements I.M
    End Sub
End Class

Class B
    Implements [|I|]
    Dim x As A
    Dim y As A
End Class",
"Interface I
    Sub M()
End Interface
Class A
    Implements I
    Public Sub M() Implements I.M
    End Sub
End Class

Class B
    Implements I
    Dim x As A
    Dim y As A
    Public Sub M() Implements I.M
        DirectCast(x, I).M()
    End Sub
End Class",
index:=1)

            Await TestAsync(
"Interface I
    Sub M()
End Interface
Class A
    Implements I
    Public Sub M() Implements I.M
    End Sub
End Class

Class B
    Implements [|I|]
    Dim x As A
    Dim y As A
End Class",
"Interface I
    Sub M()
End Interface
Class A
    Implements I
    Public Sub M() Implements I.M
    End Sub
End Class

Class B
    Implements I
    Dim x As A
    Dim y As A
    Public Sub M() Implements I.M
        DirectCast(y, I).M()
    End Sub
End Class",
index:=2)
        End Function

        <WorkItem(768799, "http://vstfdevdiv:8080/DevDiv2/DevDiv/_workitems/edit/768799")>
        <Fact, Trait(Traits.Feature, Traits.Features.CodeActionsImplementInterface)>
        Public Async Function TestImplementInterfaceThroughField_MultipleFieldsForMultipleInterfaces() As Task
            Await TestActionCountAsync(
"Interface I
    Sub M()
End Interface
Interface I2
    Sub M2()
End Interface
Class A
    Implements I
    Public Sub M() Implements I.M
    End Sub
End Class

Class B
    Implements I2
    Public Sub M2() Implements I2.M2
    End Sub
End Class

Class C
    Implements [|I|]
    Implements I2
    Dim x As A
    Dim y as B
End Class",
count:=2)

            Await TestActionCountAsync(
"Interface I
    Sub M()
End Interface
Interface I2
    Sub M2()
End Interface
Class A
    Implements I
    Public Sub M() Implements I.M
    End Sub
End Class

Class B
    Implements I2
    Public Sub M2() Implements I2.M2
    End Sub
End Class

Class C
    Implements I
    Implements [|I2|]
    Dim x As A
    Dim y as B
End Class",
count:=2)

            Await TestAsync(
"Interface I
    Sub M()
End Interface
Interface I2
    Sub M2()
End Interface
Class A
    Implements I
    Public Sub M() Implements I.M
    End Sub
End Class

Class B
    Implements I2
    Public Sub M2() Implements I2.M2
    End Sub
End Class

Class C
    Implements [|I|]
    Implements I2
    Dim x As A
    Dim y as B
End Class",
"Interface I
    Sub M()
End Interface
Interface I2
    Sub M2()
End Interface
Class A
    Implements I
    Public Sub M() Implements I.M
    End Sub
End Class

Class B
    Implements I2
    Public Sub M2() Implements I2.M2
    End Sub
End Class

Class C
    Implements I
    Implements I2
    Dim x As A
    Dim y as B
    Public Sub M() Implements I.M
        DirectCast(x, I).M()
    End Sub
End Class",
index:=1)

            Await TestAsync(
"Interface I
    Sub M()
End Interface
Interface I2
    Sub M2()
End Interface
Class A
    Implements I
    Public Sub M() Implements I.M
    End Sub
End Class

Class B
    Implements I2
    Public Sub M2() Implements I2.M2
    End Sub
End Class

Class C
    Implements I
    Implements [|I2|]
    Dim x As A
    Dim y as B
End Class",
"Interface I
    Sub M()
End Interface
Interface I2
    Sub M2()
End Interface
Class A
    Implements I
    Public Sub M() Implements I.M
    End Sub
End Class

Class B
    Implements I2
    Public Sub M2() Implements I2.M2
    End Sub
End Class

Class C
    Implements I
    Implements I2
    Dim x As A
    Dim y as B
    Public Sub M2() Implements I2.M2
        DirectCast(y, I2).M2()
    End Sub
End Class",
index:=1)
        End Function

        <WorkItem(768799, "http://vstfdevdiv:8080/DevDiv2/DevDiv/_workitems/edit/768799")>
        <Fact, Trait(Traits.Feature, Traits.Features.CodeActionsImplementInterface)>
        Public Async Function TestNoImplementThroughDefaultProperty() As Task
            Await TestActionCountAsync(
"Interface I
    Sub M()
End Interface
Class A
    Implements I
    Public Sub M() Implements I.M
    End Sub
End Class

Class B
    Implements [|I|]
    Default ReadOnly Property x(index as Integer) As A
        Get
            Return Nothing
        End Get
    End Property
End Class",
count:=1)
        End Function

        <WorkItem(768799, "http://vstfdevdiv:8080/DevDiv2/DevDiv/_workitems/edit/768799")>
        <Fact, Trait(Traits.Feature, Traits.Features.CodeActionsImplementInterface)>
        Public Async Function TestNoImplementThroughParameterizedProperty() As Task
            Await TestActionCountAsync(
"Interface I
    Sub M()
End Interface
Class A
    Implements I
    Public Sub M() Implements I.M
    End Sub
End Class

Class B
    Implements [|I|]
    ReadOnly Property x(index as Integer) As A
        Get
            Return Nothing
        End Get
    End Property
End Class",
count:=1)
        End Function

        <WorkItem(768799, "http://vstfdevdiv:8080/DevDiv2/DevDiv/_workitems/edit/768799")>
        <Fact, Trait(Traits.Feature, Traits.Features.CodeActionsImplementInterface)>
        Public Async Function TestNoImplementThroughWriteOnlyProperty() As Task
            Await TestActionCountAsync(
"Interface I
    Sub M()
End Interface
Class A
    Implements I
    Public Sub M() Implements I.M
    End Sub
End Class

Class B
    Implements [|I|]
    WriteOnly Property x(index as Integer) As A
        Set(value as A)
        End Set
    End Property
End Class",
count:=1)
        End Function

        <WorkItem(540469, "http://vstfdevdiv:8080/DevDiv2/DevDiv/_workitems/edit/540469")>
        <Fact, Trait(Traits.Feature, Traits.Features.CodeActionsImplementInterface)>
        Public Async Function TestInsertBlankLineAfterImplementsAndInherits() As Task
            Await TestAsync(
<Text>Interface I1
    Function Foo()
End Interface

Class M
    Implements [|I1|]
End Class</Text>.Value.Replace(vbLf, vbCrLf),
<Text>Imports System

Interface I1
    Function Foo()
End Interface

Class M
    Implements I1

    Public Function Foo() As Object Implements I1.Foo
        Throw New NotImplementedException()
    End Function
End Class</Text>.Value.Replace(vbLf, vbCrLf),
compareTokens:=False)
        End Function

        <WorkItem(542290, "http://vstfdevdiv:8080/DevDiv2/DevDiv/_workitems/edit/542290")>
        <Fact, Trait(Traits.Feature, Traits.Features.CodeActionsImplementInterface)>
        Public Async Function TestMethodShadowsProperty() As Task
            Await TestAsync(
"Interface I
    Sub M()
End Interface
Class B
    'Protected m As Integer 
    Public Property M As Integer
End Class
Class C
    Inherits B
    Implements [|I|]
End Class",
"Imports System
Interface I
    Sub M()
End Interface
Class B
    'Protected m As Integer 
    Public Property M As Integer
End Class
Class C
    Inherits B
    Implements I
    Private Sub I_M() Implements I.M
        Throw New NotImplementedException()
    End Sub
End Class")
        End Function

        <WorkItem(542606, "http://vstfdevdiv:8080/DevDiv2/DevDiv/_workitems/edit/542606")>
        <Fact, Trait(Traits.Feature, Traits.Features.CodeActionsImplementInterface)>
        Public Async Function TestRemMethod() As Task
            Await TestAsync(
"Interface I
    Sub [Rem]
End Interface
Class C
    Implements [|I|] ' Implement interface 
End Class",
"Imports System
Interface I
    Sub [Rem]
End Interface
Class C
    Implements I ' Implement interface 
    Public Sub [Rem]() Implements I.[Rem]
        Throw New NotImplementedException()
    End Sub
End Class")
        End Function

        <WorkItem(543425, "http://vstfdevdiv:8080/DevDiv2/DevDiv/_workitems/edit/543425")>
        <Fact, Trait(Traits.Feature, Traits.Features.CodeActionsImplementInterface)>
        Public Async Function TestMissingIfEventAlreadyImplemented() As Task
            Await TestMissingAsync(
"Imports System.ComponentModel
Class C
    Implements [|INotifyPropertyChanged|]
    Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged
End Class")
        End Function

        <WorkItem(543506, "http://vstfdevdiv:8080/DevDiv2/DevDiv/_workitems/edit/543506")>
        <Fact, Trait(Traits.Feature, Traits.Features.CodeActionsImplementInterface)>
        Public Async Function TestAddEvent1() As Task
            Await TestAsync(
"Imports System.ComponentModel
Class C
    Implements [|INotifyPropertyChanged|]
End Class",
"Imports System.ComponentModel
Class C
    Implements INotifyPropertyChanged
    Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged
End Class")
        End Function

        <WorkItem(543588, "http://vstfdevdiv:8080/DevDiv2/DevDiv/_workitems/edit/543588")>
        <Fact, Trait(Traits.Feature, Traits.Features.CodeActionsImplementInterface)>
        Public Async Function TestNameSimplifyGenericType() As Task
            Await TestAsync(
"Interface I(Of In T, Out R)
    Sub Foo()
End Interface
Class C(Of T, R)
    Implements [|I(Of T, R)|]
End Class",
"Imports System
Interface I(Of In T, Out R)
    Sub Foo()
End Interface
Class C(Of T, R)
    Implements I(Of T, R)
    Public Sub Foo() Implements I(Of T, R).Foo
        Throw New NotImplementedException()
    End Sub
End Class")
        End Function

        <WorkItem(544156, "http://vstfdevdiv:8080/DevDiv2/DevDiv/_workitems/edit/544156")>
        <Fact, Trait(Traits.Feature, Traits.Features.CodeActionsImplementInterface)>
        Public Async Function TestInterfacePropertyRedefinition() As Task
            Await TestAsync(
"Interface I1
    Property Bar As Integer
End Interface
Interface I2
    Inherits I1
    Property Bar As Integer
End Interface
Class C
    Implements [|I2|]
End Class",
"Imports System
Interface I1
    Property Bar As Integer
End Interface
Interface I2
    Inherits I1
    Property Bar As Integer
End Interface
Class C
    Implements I2
    Public Property Bar As Integer Implements I2.Bar
        Get
            Throw New NotImplementedException()
        End Get
        Set(value As Integer)
            Throw New NotImplementedException()
        End Set
    End Property
    Private Property I1_Bar As Integer Implements I1.Bar
        Get
            Throw New NotImplementedException()
        End Get
        Set(value As Integer)
            Throw New NotImplementedException()
        End Set
    End Property
End Class")
        End Function

        <WorkItem(544208, "http://vstfdevdiv:8080/DevDiv2/DevDiv/_workitems/edit/544208")>
        <Fact, Trait(Traits.Feature, Traits.Features.CodeActionsImplementInterface)>
        Public Async Function TestMissingOnWrongArity() As Task
            Await TestMissingAsync(
"Interface I1(Of T)
    ReadOnly Property Bar As Integer
End Interface
Class C
    Implements [|I1|]
End Class")
        End Function

        <WorkItem(529328, "http://vstfdevdiv:8080/DevDiv2/DevDiv/_workitems/edit/529328")>
        <Fact, Trait(Traits.Feature, Traits.Features.CodeActionsImplementInterface)>
        Public Async Function TestPropertyShadowing() As Task
            Await TestAsync(
"Interface I1
    Property Bar As Integer
    Sub Foo()
End Interface
Class B
    Public Property Bar As Integer
End Class
Class C
    Inherits B
    Implements [|I1|]
End Class",
"Imports System
Interface I1
    Property Bar As Integer
    Sub Foo()
End Interface
Class B
    Public Property Bar As Integer
End Class
Class C
    Inherits B
    Implements I1
    Private Property I1_Bar As Integer Implements I1.Bar
        Get
            Throw New NotImplementedException()
        End Get
        Set(value As Integer)
            Throw New NotImplementedException()
        End Set
    End Property
    Public Sub Foo() Implements I1.Foo
        Throw New NotImplementedException()
    End Sub
End Class")
        End Function

        <WorkItem(544206, "http://vstfdevdiv:8080/DevDiv2/DevDiv/_workitems/edit/544206")>
        <Fact, Trait(Traits.Feature, Traits.Features.CodeActionsImplementInterface)>
        Public Async Function TestEventWithImplicitDelegateCreation() As Task
            Await TestAsync(
"Interface I1
    Event E(x As String)
End Interface
Class C
    Implements [|I1|]
End Class",
"Interface I1
    Event E(x As String)
End Interface
Class C
    Implements I1
    Public Event E(x As String) Implements I1.E
End Class")
        End Function

        <Fact, Trait(Traits.Feature, Traits.Features.CodeActionsImplementInterface)>
        Public Async Function TestStringLiteral() As Task
            Await TestAsync(
"Interface IFoo
    Sub Foo(Optional s As String = """""""")
End Interface
Class Bar
    Implements [|IFoo|]
End Class",
"Imports System
Interface IFoo
    Sub Foo(Optional s As String = """""""")
End Interface
Class Bar
    Implements IFoo
    Public Sub Foo(Optional s As String = """""""") Implements IFoo.Foo
        Throw New NotImplementedException()
    End Sub
End Class")
        End Function

        <WorkItem(545643, "http://vstfdevdiv:8080/DevDiv2/DevDiv/_workitems/edit/545643"), WorkItem(715013, "http://vstfdevdiv:8080/DevDiv2/DevDiv/_workitems/edit/715013")>
        <Fact(), Trait(Traits.Feature, Traits.Features.CodeActionsImplementInterface)>
        Public Async Function TestVBConstantValue1() As Task
            Await TestAsync(
"Imports System
Imports Microsoft.VisualBasic
Interface I
    Sub VBNullChar(Optional x As String = Constants.vbNullChar)
End Interface

Class C
    Implements [|I|]
End Class",
"Imports System
Imports Microsoft.VisualBasic
Interface I
    Sub VBNullChar(Optional x As String = Constants.vbNullChar)
End Interface

Class C
    Implements I
    Public Sub VBNullChar(Optional x As String = Constants.vbNullChar) Implements I.VBNullChar
        Throw New NotImplementedException()
    End Sub
End Class")
        End Function

        <Fact, Trait(Traits.Feature, Traits.Features.CodeActionsImplementInterface)>
        Public Async Function TestVBConstantValue2() As Task
            Await TestAsync(
"Imports System
Imports Microsoft.VisualBasic
Interface I
    Sub VBNullChar(Optional x As String = Constants.vbNullChar)
End Interface

Namespace N
    Class Microsoft
        Implements [|I|]
    End Class
End Namespace",
"Imports System
Imports Microsoft.VisualBasic
Interface I
    Sub VBNullChar(Optional x As String = Constants.vbNullChar)
End Interface

Namespace N
    Class Microsoft
        Implements I
        Public Sub VBNullChar(Optional x As String = Constants.vbNullChar) Implements I.VBNullChar
            Throw New NotImplementedException()
        End Sub
    End Class
End Namespace",
parseOptions:=Nothing)
        End Function

        <WorkItem(545679, "http://vstfdevdiv:8080/DevDiv2/DevDiv/_workitems/edit/545679"), WorkItem(715013, "http://vstfdevdiv:8080/DevDiv2/DevDiv/_workitems/edit/715013")>
        <Fact(), Trait(Traits.Feature, Traits.Features.CodeActionsImplementInterface)>
        Public Async Function TestVBConstantValue3() As Task
            Await TestAsync(
"Imports System
Imports Microsoft.VisualBasic
Interface I
    Sub ChrW(Optional x As String = Strings.ChrW(1))
End Interface

Class C
    Implements [|I|]
End Class",
"Imports System
Imports Microsoft.VisualBasic
Interface I
    Sub ChrW(Optional x As String = Strings.ChrW(1))
End Interface

Class C
    Implements I
    Public Sub ChrW(Optional x As String = Strings.ChrW(1)) Implements I.ChrW
        Throw New NotImplementedException()
    End Sub
End Class")
        End Function

        <WorkItem(545674, "http://vstfdevdiv:8080/DevDiv2/DevDiv/_workitems/edit/545674")>
        <Fact, Trait(Traits.Feature, Traits.Features.CodeActionsImplementInterface)>
        Public Async Function TestDateTimeLiteral1() As Task
            Await TestAsync(
"Interface I
    Sub Foo(Optional x As Date = #6/29/2012#)
End Interface
Class C
    Implements [|I|]
End Class",
"Imports System
Interface I
    Sub Foo(Optional x As Date = #6/29/2012#)
End Interface
Class C
    Implements I
    Public Sub Foo(Optional x As Date = #6/29/2012 12:00:00 AM#) Implements I.Foo
        Throw New NotImplementedException()
    End Sub
End Class")
        End Function

        <WorkItem(545675, "http://vstfdevdiv:8080/DevDiv2/DevDiv/_workitems/edit/545675"), WorkItem(715013, "http://vstfdevdiv:8080/DevDiv2/DevDiv/_workitems/edit/715013")>
        <Fact(), Trait(Traits.Feature, Traits.Features.CodeActionsImplementInterface)>
        Public Async Function TestEnumConstant1() As Task
            Await TestAsync(
"Option Strict On
Imports System
Interface I
    Sub Foo(Optional x As DayOfWeek = DayOfWeek.Friday)
End Interface

Class C
    Implements [|I|]
End Class",
"Option Strict On
Imports System
Interface I
    Sub Foo(Optional x As DayOfWeek = DayOfWeek.Friday)
End Interface

Class C
    Implements I
    Public Sub Foo(Optional x As DayOfWeek = DayOfWeek.Friday) Implements I.Foo
        Throw New NotImplementedException()
    End Sub
End Class")
        End Function

        <WorkItem(545644, "http://vstfdevdiv:8080/DevDiv2/DevDiv/_workitems/edit/545644")>
        <Fact, Trait(Traits.Feature, Traits.Features.CodeActionsImplementInterface)>
        Public Async Function TestMultiDimensionalArray1() As Task
            Await TestAsync(
"Interface I
    Sub Foo(x As Integer()())
End Interface

Class C
    Implements [|I|]
End Class",
"Imports System
Interface I
    Sub Foo(x As Integer()())
End Interface

Class C
    Implements I
    Public Sub Foo(x As Integer()()) Implements I.Foo
        Throw New NotImplementedException()
    End Sub
End Class")
        End Function

        <WorkItem(545640, "http://vstfdevdiv:8080/DevDiv2/DevDiv/_workitems/edit/545640"), WorkItem(715013, "http://vstfdevdiv:8080/DevDiv2/DevDiv/_workitems/edit/715013")>
        <Fact(), Trait(Traits.Feature, Traits.Features.CodeActionsImplementInterface)>
        Public Async Function TestUnicodeQuote() As Task
            Await TestAsync(
"Imports System
Imports Microsoft.VisualBasic
Interface I
    Sub Foo(Optional x As String = ChrW(8220))
End Interface

Class C
    Implements [|I|]
End Class",
"Imports System
Imports Microsoft.VisualBasic
Interface I
    Sub Foo(Optional x As String = ChrW(8220))
End Interface

Class C
    Implements I
    Public Sub Foo(Optional x As String = ChrW(8220)) Implements I.Foo
        Throw New NotImplementedException()
    End Sub
End Class")
        End Function

        <WorkItem(545563, "http://vstfdevdiv:8080/DevDiv2/DevDiv/_workitems/edit/545563")>
        <Fact, Trait(Traits.Feature, Traits.Features.CodeActionsImplementInterface)>
        Public Async Function TestLongMinValue() As Task
            Await TestAsync(
"Interface I
    Sub Foo(Optional x As Long = Long.MinValue)
End Interface

Class C
    Implements [|I|]
End Class",
"Imports System
Interface I
    Sub Foo(Optional x As Long = Long.MinValue)
End Interface

Class C
    Implements I
    Public Sub Foo(Optional x As Long = Long.MinValue) Implements I.Foo
        Throw New NotImplementedException()
    End Sub
End Class")
        End Function

        <Fact, Trait(Traits.Feature, Traits.Features.CodeActionsImplementInterface)>
        Public Async Function TestMinMaxValues() As Task
            Await TestAsync(
<Text>Interface I
    Sub M01(Optional x As Short = Short.MinValue)
    Sub M02(Optional x As Short = Short.MaxValue)
    Sub M03(Optional x As UShort = UShort.MinValue)
    Sub M04(Optional x As UShort = UShort.MaxValue)
    Sub M05(Optional x As Integer = Integer.MinValue)
    Sub M06(Optional x As Integer = Integer.MaxValue)
    Sub M07(Optional x As UInteger = UInteger.MinValue)
    Sub M08(Optional x As UInteger = UInteger.MaxValue)
    Sub M09(Optional x As Long = Long.MinValue)
    Sub M10(Optional x As Long = Long.MaxValue)
    Sub M11(Optional x As ULong = ULong.MinValue)
    Sub M12(Optional x As ULong = ULong.MaxValue)
End Interface

Class C
    Implements [|I|]
End Class</Text>.Value.Replace(vbLf, vbCrLf),
<Text>Imports System

Interface I
    Sub M01(Optional x As Short = Short.MinValue)
    Sub M02(Optional x As Short = Short.MaxValue)
    Sub M03(Optional x As UShort = UShort.MinValue)
    Sub M04(Optional x As UShort = UShort.MaxValue)
    Sub M05(Optional x As Integer = Integer.MinValue)
    Sub M06(Optional x As Integer = Integer.MaxValue)
    Sub M07(Optional x As UInteger = UInteger.MinValue)
    Sub M08(Optional x As UInteger = UInteger.MaxValue)
    Sub M09(Optional x As Long = Long.MinValue)
    Sub M10(Optional x As Long = Long.MaxValue)
    Sub M11(Optional x As ULong = ULong.MinValue)
    Sub M12(Optional x As ULong = ULong.MaxValue)
End Interface

Class C
    Implements I

    Public Sub M01(Optional x As Short = Short.MinValue) Implements I.M01
        Throw New NotImplementedException()
    End Sub

    Public Sub M02(Optional x As Short = Short.MaxValue) Implements I.M02
        Throw New NotImplementedException()
    End Sub

    Public Sub M03(Optional x As UShort = 0) Implements I.M03
        Throw New NotImplementedException()
    End Sub

    Public Sub M04(Optional x As UShort = UShort.MaxValue) Implements I.M04
        Throw New NotImplementedException()
    End Sub

    Public Sub M05(Optional x As Integer = Integer.MinValue) Implements I.M05
        Throw New NotImplementedException()
    End Sub

    Public Sub M06(Optional x As Integer = Integer.MaxValue) Implements I.M06
        Throw New NotImplementedException()
    End Sub

    Public Sub M07(Optional x As UInteger = 0) Implements I.M07
        Throw New NotImplementedException()
    End Sub

    Public Sub M08(Optional x As UInteger = UInteger.MaxValue) Implements I.M08
        Throw New NotImplementedException()
    End Sub

    Public Sub M09(Optional x As Long = Long.MinValue) Implements I.M09
        Throw New NotImplementedException()
    End Sub

    Public Sub M10(Optional x As Long = Long.MaxValue) Implements I.M10
        Throw New NotImplementedException()
    End Sub

    Public Sub M11(Optional x As ULong = 0) Implements I.M11
        Throw New NotImplementedException()
    End Sub

    Public Sub M12(Optional x As ULong = ULong.MaxValue) Implements I.M12
        Throw New NotImplementedException()
    End Sub
End Class</Text>.Value.Replace(vbLf, vbCrLf),
compareTokens:=False)
        End Function

        <Fact, Trait(Traits.Feature, Traits.Features.CodeActionsImplementInterface)>
        Public Async Function TestFloatConstants() As Task
            Await TestAsync(
<Text>Interface I
    Sub D1(Optional x As Double = Double.Epsilon)
    Sub D2(Optional x As Double = Double.MaxValue)
    Sub D3(Optional x As Double = Double.MinValue)
    Sub D4(Optional x As Double = Double.NaN)
    Sub D5(Optional x As Double = Double.NegativeInfinity)
    Sub D6(Optional x As Double = Double.PositiveInfinity)
    Sub S1(Optional x As Single = Single.Epsilon)
    Sub S2(Optional x As Single = Single.MaxValue)
    Sub S3(Optional x As Single = Single.MinValue)
    Sub S4(Optional x As Single = Single.NaN)
    Sub S5(Optional x As Single = Single.NegativeInfinity)
    Sub S6(Optional x As Single = Single.PositiveInfinity)
End Interface

Class C
    Implements [|I|]
End Class</Text>.Value.Replace(vbLf, vbCrLf),
<Text>Imports System

Interface I
    Sub D1(Optional x As Double = Double.Epsilon)
    Sub D2(Optional x As Double = Double.MaxValue)
    Sub D3(Optional x As Double = Double.MinValue)
    Sub D4(Optional x As Double = Double.NaN)
    Sub D5(Optional x As Double = Double.NegativeInfinity)
    Sub D6(Optional x As Double = Double.PositiveInfinity)
    Sub S1(Optional x As Single = Single.Epsilon)
    Sub S2(Optional x As Single = Single.MaxValue)
    Sub S3(Optional x As Single = Single.MinValue)
    Sub S4(Optional x As Single = Single.NaN)
    Sub S5(Optional x As Single = Single.NegativeInfinity)
    Sub S6(Optional x As Single = Single.PositiveInfinity)
End Interface

Class C
    Implements I

    Public Sub D1(Optional x As Double = Double.Epsilon) Implements I.D1
        Throw New NotImplementedException()
    End Sub

    Public Sub D2(Optional x As Double = Double.MaxValue) Implements I.D2
        Throw New NotImplementedException()
    End Sub

    Public Sub D3(Optional x As Double = Double.MinValue) Implements I.D3
        Throw New NotImplementedException()
    End Sub

    Public Sub D4(Optional x As Double = Double.NaN) Implements I.D4
        Throw New NotImplementedException()
    End Sub

    Public Sub D5(Optional x As Double = Double.NegativeInfinity) Implements I.D5
        Throw New NotImplementedException()
    End Sub

    Public Sub D6(Optional x As Double = Double.PositiveInfinity) Implements I.D6
        Throw New NotImplementedException()
    End Sub

    Public Sub S1(Optional x As Single = Single.Epsilon) Implements I.S1
        Throw New NotImplementedException()
    End Sub

    Public Sub S2(Optional x As Single = Single.MaxValue) Implements I.S2
        Throw New NotImplementedException()
    End Sub

    Public Sub S3(Optional x As Single = Single.MinValue) Implements I.S3
        Throw New NotImplementedException()
    End Sub

    Public Sub S4(Optional x As Single = Single.NaN) Implements I.S4
        Throw New NotImplementedException()
    End Sub

    Public Sub S5(Optional x As Single = Single.NegativeInfinity) Implements I.S5
        Throw New NotImplementedException()
    End Sub

    Public Sub S6(Optional x As Single = Single.PositiveInfinity) Implements I.S6
        Throw New NotImplementedException()
    End Sub
End Class</Text>.Value.Replace(vbLf, vbCrLf),
compareTokens:=False)
        End Function

        <WorkItem(715013, "http://vstfdevdiv:8080/DevDiv2/DevDiv/_workitems/edit/715013")>
        <Fact(), Trait(Traits.Feature, Traits.Features.CodeActionsImplementInterface)>
        Public Async Function TestEnumParameters() As Task
            Await TestAsync(
<Text><![CDATA[Imports System

Enum E
    A = 1
    B = 2
End Enum

<FlagsAttribute>
Enum FlagE
    A = 1
    B = 2
End Enum

Interface I
    Sub M1(Optional e As E = E.A Or E.B)
    Sub M2(Optional e As FlagE = FlagE.A Or FlagE.B)
End Interface

Class C
    Implements [|I|]
End Class]]></Text>.Value.Replace(vbLf, vbCrLf),
<Text><![CDATA[Imports System

Enum E
    A = 1
    B = 2
End Enum

<FlagsAttribute>
Enum FlagE
    A = 1
    B = 2
End Enum

Interface I
    Sub M1(Optional e As E = E.A Or E.B)
    Sub M2(Optional e As FlagE = FlagE.A Or FlagE.B)
End Interface

Class C
    Implements I

    Public Sub M1(Optional e As E = 3) Implements I.M1
        Throw New NotImplementedException()
    End Sub

    Public Sub M2(Optional e As FlagE = FlagE.A Or FlagE.B) Implements I.M2
        Throw New NotImplementedException()
    End Sub
End Class]]></Text>.Value.Replace(vbLf, vbCrLf),
compareTokens:=False)
        End Function

        <WorkItem(715013, "http://vstfdevdiv:8080/DevDiv2/DevDiv/_workitems/edit/715013")>
        <Fact(), Trait(Traits.Feature, Traits.Features.CodeActionsImplementInterface)>
        Public Async Function TestEnumParameters2() As Task
            Await TestAsync(
<Text><![CDATA[
Option Strict On
Imports System

Enum E
    A = 1
    B = 2
End Enum

<FlagsAttribute>
Enum FlagE
    A = 1
    B = 2
End Enum

Interface I
    Sub M1(Optional e As E = E.A Or E.B)
    Sub M2(Optional e As FlagE = FlagE.A Or FlagE.B)
End Interface

Class C
    Implements [|I|]
End Class]]></Text>.Value.Replace(vbLf, vbCrLf),
<Text><![CDATA[
Option Strict On
Imports System

Enum E
    A = 1
    B = 2
End Enum

<FlagsAttribute>
Enum FlagE
    A = 1
    B = 2
End Enum

Interface I
    Sub M1(Optional e As E = E.A Or E.B)
    Sub M2(Optional e As FlagE = FlagE.A Or FlagE.B)
End Interface

Class C
    Implements I

    Public Sub M1(Optional e As E = CType(3, E)) Implements I.M1
        Throw New NotImplementedException()
    End Sub

    Public Sub M2(Optional e As FlagE = FlagE.A Or FlagE.B) Implements I.M2
        Throw New NotImplementedException()
    End Sub
End Class]]></Text>.Value.Replace(vbLf, vbCrLf),
compareTokens:=False)
        End Function

        <WorkItem(545691, "http://vstfdevdiv:8080/DevDiv2/DevDiv/_workitems/edit/545691")>
        <Fact, Trait(Traits.Feature, Traits.Features.CodeActionsImplementInterface)>
        Public Async Function TestMultiDimArray1() As Task
            Await TestAsync(
"Interface I
    Sub Foo(x As Integer(,))
End Interface

Class C
    Implements [|I|]
End Class",
"Imports System
Interface I
    Sub Foo(x As Integer(,))
End Interface

Class C
    Implements I
    Public Sub Foo(x(,) As Integer) Implements I.Foo
        Throw New NotImplementedException()
    End Sub
End Class")
        End Function

        <WorkItem(545640, "http://vstfdevdiv:8080/DevDiv2/DevDiv/_workitems/edit/545640"), WorkItem(715013, "http://vstfdevdiv:8080/DevDiv2/DevDiv/_workitems/edit/715013")>
        <Fact(), Trait(Traits.Feature, Traits.Features.CodeActionsImplementInterface)>
        Public Async Function TestQuoteEscaping1() As Task
            Await TestAsync(
"Imports System
Imports Microsoft.VisualBasic
Interface I
    Sub Foo(Optional x As Char = ChrW(8220))
End Interface
Class C
    Implements [|I|]
End Class",
"Imports System
Imports Microsoft.VisualBasic
Interface I
    Sub Foo(Optional x As Char = ChrW(8220))
End Interface
Class C
    Implements I
    Public Sub Foo(Optional x As Char = ChrW(8220)) Implements I.Foo
        Throw New NotImplementedException()
    End Sub
End Class")
        End Function

        <WorkItem(545866, "http://vstfdevdiv:8080/DevDiv2/DevDiv/_workitems/edit/545866")>
        <Fact, Trait(Traits.Feature, Traits.Features.CodeActionsImplementInterface)>
        Public Async Function TestQuoteEscaping2() As Task
            Await TestAsync(
"Interface I
    Sub Foo(Optional x As Object = ""‟"")
End Interface

Class C
    Implements [|I|]
End Class",
"Imports System
Interface I
    Sub Foo(Optional x As Object = ""‟"")
End Interface

Class C
    Implements I
    Public Sub Foo(Optional x As Object = ""‟"") Implements I.Foo
        Throw New NotImplementedException()
    End Sub
End Class")
        End Function

        <WorkItem(545689, "http://vstfdevdiv:8080/DevDiv2/DevDiv/_workitems/edit/545689")>
        <Fact, Trait(Traits.Feature, Traits.Features.CodeActionsImplementInterface)>
        Public Async Function TestDecimalLiteral1() As Task
            Await TestAsync(
"Option Strict On
Imports System
Interface I
    Sub Foo(Optional x As Decimal = Decimal.MaxValue)
End Interface

Class C
    Implements [|I|]
End Class",
"Option Strict On
Imports System
Interface I
    Sub Foo(Optional x As Decimal = Decimal.MaxValue)
End Interface

Class C
    Implements I
    Public Sub Foo(Optional x As Decimal = Decimal.MaxValue) Implements I.Foo
        Throw New NotImplementedException()
    End Sub
End Class")
        End Function

        <WorkItem(545687, "http://vstfdevdiv:8080/DevDiv2/DevDiv/_workitems/edit/545687"), WorkItem(715013, "http://vstfdevdiv:8080/DevDiv2/DevDiv/_workitems/edit/715013")>
        <Fact(), Trait(Traits.Feature, Traits.Features.CodeActionsImplementInterface)>
        Public Async Function TestRemoveParenthesesAroundTypeReference1() As Task
            Await TestAsync(
"Option Strict On
Imports System
Interface I
    Sub Foo(Optional x As DayOfWeek = DayOfWeek.Monday)
End Interface

Class C
    Implements [|I|]

    Property DayOfWeek As DayOfWeek
End Class",
"Option Strict On
Imports System
Interface I
    Sub Foo(Optional x As DayOfWeek = DayOfWeek.Monday)
End Interface

Class C
    Implements I

    Property DayOfWeek As DayOfWeek
    Public Sub Foo(Optional x As DayOfWeek = DayOfWeek.Monday) Implements I.Foo
        Throw New NotImplementedException()
    End Sub
End Class")
        End Function

        <WorkItem(545694, "http://vstfdevdiv:8080/DevDiv2/DevDiv/_workitems/edit/545694"), WorkItem(715013, "http://vstfdevdiv:8080/DevDiv2/DevDiv/_workitems/edit/715013")>
        <Fact(), Trait(Traits.Feature, Traits.Features.CodeActionsImplementInterface)>
        Public Async Function TestNullableDefaultValue1() As Task
            Await TestAsync(
"Option Strict On
Imports System
Interface I
    Sub Foo(Optional x As DayOfWeek? = DayOfWeek.Friday)
End Interface

Class C
    Implements [|I|]
End Class",
"Option Strict On
Imports System
Interface I
    Sub Foo(Optional x As DayOfWeek? = DayOfWeek.Friday)
End Interface

Class C
    Implements I
    Public Sub Foo(Optional x As DayOfWeek? = DayOfWeek.Friday) Implements I.Foo
        Throw New NotImplementedException()
    End Sub
End Class")
        End Function

        <WorkItem(545688, "http://vstfdevdiv:8080/DevDiv2/DevDiv/_workitems/edit/545688")>
        <Fact, Trait(Traits.Feature, Traits.Features.CodeActionsImplementInterface)>
        Public Async Function TestHighPrecisionDouble() As Task
            Await TestAsync(
"Imports System
Interface I
    Sub Foo(Optional x As Double = 2.8025969286496341E-45)
End Interface

Class C
    Implements [|I|]
End Class",
"Imports System
Interface I
    Sub Foo(Optional x As Double = 2.8025969286496341E-45)
End Interface

Class C
    Implements I
    Public Sub Foo(Optional x As Double = 2.8025969286496341E-45) Implements I.Foo
        Throw New NotImplementedException()
    End Sub
End Class")
        End Function

        <WorkItem(545729, "http://vstfdevdiv:8080/DevDiv2/DevDiv/_workitems/edit/545729"), WorkItem(715013, "http://vstfdevdiv:8080/DevDiv2/DevDiv/_workitems/edit/715013")>
        <Fact(), Trait(Traits.Feature, Traits.Features.CodeActionsImplementInterface)>
        Public Async Function TestCharSurrogates() As Task
            Await TestAsync(
"Imports System
Imports Microsoft.VisualBasic
Interface I
    Sub Foo(Optional x As Char = ChrW(55401))
End Interface

Class C
    Implements [|I|]
End Class",
"Imports System
Imports Microsoft.VisualBasic
Interface I
    Sub Foo(Optional x As Char = ChrW(55401))
End Interface

Class C
    Implements I
    Public Sub Foo(Optional x As Char = ChrW(55401)) Implements I.Foo
        Throw New NotImplementedException()
    End Sub
End Class")
        End Function

        <WorkItem(545733, "http://vstfdevdiv:8080/DevDiv2/DevDiv/_workitems/edit/545733"), WorkItem(715013, "http://vstfdevdiv:8080/DevDiv2/DevDiv/_workitems/edit/715013")>
        <Fact(), Trait(Traits.Feature, Traits.Features.CodeActionsImplementInterface)>
        Public Async Function TestReservedChar() As Task
            Await TestAsync(
"Option Strict On
Imports System
Imports Microsoft.VisualBasic
Interface I
    Sub Foo(Optional x As Char = Chr(13))
End Interface

Class C
    Implements [|I|]
End Class",
"Option Strict On
Imports System
Imports Microsoft.VisualBasic
Interface I
    Sub Foo(Optional x As Char = Chr(13))
End Interface

Class C
    Implements I
    Public Sub Foo(Optional x As Char = ChrW(13)) Implements I.Foo
        Throw New NotImplementedException()
    End Sub
End Class")
        End Function

        <WorkItem(545685, "http://vstfdevdiv:8080/DevDiv2/DevDiv/_workitems/edit/545685")>
        <Fact, Trait(Traits.Feature, Traits.Features.CodeActionsImplementInterface)>
        Public Async Function TestCastEnumValue() As Task
            Await TestAsync(
"Option Strict On
Imports System
Interface I
    Sub Foo(Optional x As ConsoleColor = CType(-1, ConsoleColor))
End Interface

Class C
    Implements [|I|]
End Class",
"Option Strict On
Imports System
Interface I
    Sub Foo(Optional x As ConsoleColor = CType(-1, ConsoleColor))
End Interface

Class C
    Implements I
    Public Sub Foo(Optional x As ConsoleColor = CType(-1, ConsoleColor)) Implements I.Foo
        Throw New NotImplementedException()
    End Sub
End Class")
        End Function

        <WorkItem(545756, "http://vstfdevdiv:8080/DevDiv2/DevDiv/_workitems/edit/545756")>
        <Fact, Trait(Traits.Feature, Traits.Features.CodeActionsImplementInterface)>
        Public Async Function TestArrayOfNullables() As Task
            Await TestAsync(
"Interface I
    Sub Foo(x As Integer?())
End Interface

Class C
    Implements [|I|]
End Class",
"Imports System
Interface I
    Sub Foo(x As Integer?())
End Interface

Class C
    Implements I
    Public Sub Foo(x As Integer?()) Implements I.Foo
        Throw New NotImplementedException()
    End Sub
End Class")
        End Function

        <WorkItem(545753, "http://vstfdevdiv:8080/DevDiv2/DevDiv/_workitems/edit/545753")>
        <Fact, Trait(Traits.Feature, Traits.Features.CodeActionsImplementInterface)>
        Public Async Function TestOptionalArrayParameterWithDefault() As Task
            Await TestAsync(
"Interface I
    Sub Foo(Optional x As Integer() = Nothing)
End Interface

Class C
    Implements [|I|]
End Class",
"Imports System
Interface I
    Sub Foo(Optional x As Integer() = Nothing)
End Interface

Class C
    Implements I
    Public Sub Foo(Optional x() As Integer = Nothing) Implements I.Foo
        Throw New NotImplementedException()
    End Sub
End Class")
        End Function

        <WorkItem(545742, "http://vstfdevdiv:8080/DevDiv2/DevDiv/_workitems/edit/545742"), WorkItem(715013, "http://vstfdevdiv:8080/DevDiv2/DevDiv/_workitems/edit/715013")>
        <Fact(), Trait(Traits.Feature, Traits.Features.CodeActionsImplementInterface)>
        Public Async Function TestRemFieldEnum() As Task
            Await TestAsync(
"Option Strict On
Imports System
Enum E
    [Rem]
End Enum

Interface I
    Sub Foo(Optional x As E = E.[Rem])
End Interface

Class C
    Implements [|I|]
End Class",
"Option Strict On
Imports System
Enum E
    [Rem]
End Enum

Interface I
    Sub Foo(Optional x As E = E.[Rem])
End Interface

Class C
    Implements I
    Public Sub Foo(Optional x As E = E.[Rem]) Implements I.Foo
        Throw New NotImplementedException()
    End Sub
End Class")
        End Function

        <WorkItem(545790, "http://vstfdevdiv:8080/DevDiv2/DevDiv/_workitems/edit/545790")>
        <Fact, Trait(Traits.Feature, Traits.Features.CodeActionsImplementInterface)>
        Public Async Function TestByteParameter() As Task
            Await TestAsync(
"Interface I
    Sub Foo(Optional x As Byte = 1)
End Interface

Class C
    Implements [|I|]
End Class",
"Imports System
Interface I
    Sub Foo(Optional x As Byte = 1)
End Interface

Class C
    Implements I
    Public Sub Foo(Optional x As Byte = 1) Implements I.Foo
        Throw New NotImplementedException()
    End Sub
End Class")
        End Function

        <WorkItem(545789, "http://vstfdevdiv:8080/DevDiv2/DevDiv/_workitems/edit/545789")>
        <Fact, Trait(Traits.Feature, Traits.Features.CodeActionsImplementInterface)>
        Public Async Function TestDefaultParameterSuffix1() As Task
            Await TestAsync(
"Interface I
    Sub Foo(Optional x As Object = 1L)
End Interface

Class C
    Implements [|I|]
End Class",
"Imports System
Interface I
    Sub Foo(Optional x As Object = 1L)
End Interface

Class C
    Implements I
    Public Sub Foo(Optional x As Object = 1L) Implements I.Foo
        Throw New NotImplementedException()
    End Sub
End Class")
        End Function

        <WorkItem(545809, "http://vstfdevdiv:8080/DevDiv2/DevDiv/_workitems/edit/545809")>
        <Fact, Trait(Traits.Feature, Traits.Features.CodeActionsImplementInterface)>
        Public Async Function TestZeroValuedEnum() As Task
            Await TestAsync(
"Enum E
    A = 1
End Enum

Interface I
    Sub Foo(Optional x As E = 0)
End Interface

Class C
    Implements [|I|]
End Class",
"Imports System
Enum E
    A = 1
End Enum

Interface I
    Sub Foo(Optional x As E = 0)
End Interface

Class C
    Implements I
    Public Sub Foo(Optional x As E = 0) Implements I.Foo
        Throw New NotImplementedException()
    End Sub
End Class")
        End Function

        <WorkItem(545824, "http://vstfdevdiv:8080/DevDiv2/DevDiv/_workitems/edit/545824")>
        <Fact, Trait(Traits.Feature, Traits.Features.CodeActionsImplementInterface)>
        Public Async Function TestByteCast() As Task
            Await TestAsync(
"Interface I
    Sub Foo(Optional x As Object = CByte(1))
End Interface

Class C
    Implements [|I|]
End Class",
"Imports System
Interface I
    Sub Foo(Optional x As Object = CByte(1))
End Interface

Class C
    Implements I
    Public Sub Foo(Optional x As Object = CByte(1)) Implements I.Foo
        Throw New NotImplementedException()
    End Sub
End Class")
        End Function

        <WorkItem(545825, "http://vstfdevdiv:8080/DevDiv2/DevDiv/_workitems/edit/545825")>
        <Fact, Trait(Traits.Feature, Traits.Features.CodeActionsImplementInterface)>
        Public Async Function TestDecimalValues() As Task
            Await TestAsync(
<Text>Option Strict On

Interface I
    Sub M1(Optional x As Decimal = 2D)
    Sub M2(Optional x As Decimal = 2.0D)
    Sub M3(Optional x As Decimal = 0D)
    Sub M4(Optional x As Decimal = 0.0D)
    Sub M5(Optional x As Decimal = 0.1D)
    Sub M6(Optional x As Decimal = 0.10D)
End Interface
 
Class C
    Implements [|I|]
End Class
</Text>.Value.Replace(vbLf, vbCrLf),
<Text>Option Strict On
Imports System

Interface I
    Sub M1(Optional x As Decimal = 2D)
    Sub M2(Optional x As Decimal = 2.0D)
    Sub M3(Optional x As Decimal = 0D)
    Sub M4(Optional x As Decimal = 0.0D)
    Sub M5(Optional x As Decimal = 0.1D)
    Sub M6(Optional x As Decimal = 0.10D)
End Interface
 
Class C
    Implements I

    Public Sub M1(Optional x As Decimal = 2) Implements I.M1
        Throw New NotImplementedException()
    End Sub

    Public Sub M2(Optional x As Decimal = 2.0D) Implements I.M2
        Throw New NotImplementedException()
    End Sub

    Public Sub M3(Optional x As Decimal = 0) Implements I.M3
        Throw New NotImplementedException()
    End Sub

    Public Sub M4(Optional x As Decimal = 0.0D) Implements I.M4
        Throw New NotImplementedException()
    End Sub

    Public Sub M5(Optional x As Decimal = 0.1D) Implements I.M5
        Throw New NotImplementedException()
    End Sub

    Public Sub M6(Optional x As Decimal = 0.10D) Implements I.M6
        Throw New NotImplementedException()
    End Sub
End Class
</Text>.Value.Replace(vbLf, vbCrLf))
        End Function

        <WorkItem(545693, "http://vstfdevdiv:8080/DevDiv2/DevDiv/_workitems/edit/545693")>
        <Fact, Trait(Traits.Feature, Traits.Features.CodeActionsImplementInterface)>
        Public Async Function TestSmallDecimal() As Task
            Await TestAsync(
"Option Strict On

Interface I
    Sub Foo(Optional x As Decimal = Long.MinValue)
End Interface

Class C
    Implements [|I|]
End Class",
"Option Strict On
Imports System
Interface I
    Sub Foo(Optional x As Decimal = Long.MinValue)
End Interface

Class C
    Implements I
    Public Sub Foo(Optional x As Decimal = -9223372036854775808D) Implements I.Foo
        Throw New NotImplementedException()
    End Sub
End Class")
        End Function

        <WorkItem(545771, "http://vstfdevdiv:8080/DevDiv2/DevDiv/_workitems/edit/545771")>
        <Fact, Trait(Traits.Feature, Traits.Features.CodeActionsImplementInterface)>
        Public Async Function TestEventConflict() As Task
            Await TestAsync(
"Interface IA
    Event E As EventHandler
End Interface
Interface IB
    Inherits IA
    Shadows Event E As Action
End Interface
Class C
    Implements [|IB|]
End Class",
"Interface IA
    Event E As EventHandler
End Interface
Interface IB
    Inherits IA
    Shadows Event E As Action
End Interface
Class C
    Implements IB
    Public Event E As Action Implements IB.E
    Private Event IA_E As EventHandler Implements IA.E
End Class")
        End Function

        <WorkItem(545826, "http://vstfdevdiv:8080/DevDiv2/DevDiv/_workitems/edit/545826")>
        <Fact, Trait(Traits.Feature, Traits.Features.CodeActionsImplementInterface)>
        Public Async Function TestDecimalField() As Task
            Await TestAsync(
"Option Strict On
Imports System
Interface I
    Sub Foo(Optional x As Object = 1D)
End Interface

Class C
    Implements [|I|]
End Class",
"Option Strict On
Imports System
Interface I
    Sub Foo(Optional x As Object = 1D)
End Interface

Class C
    Implements I
    Public Sub Foo(Optional x As Object = 1D) Implements I.Foo
        Throw New NotImplementedException()
    End Sub
End Class")
        End Function

        <WorkItem(545827, "http://vstfdevdiv:8080/DevDiv2/DevDiv/_workitems/edit/545827")>
        <Fact, Trait(Traits.Feature, Traits.Features.CodeActionsImplementInterface)>
        Public Async Function TestDoubleInObjectContext() As Task
            Await TestAsync(
"Option Strict On
Imports System
Interface I
    Sub Foo(Optional x As Object = 1.0)
End Interface

Class C
    Implements [|I|]
End Class",
"Option Strict On
Imports System
Interface I
    Sub Foo(Optional x As Object = 1.0)
End Interface

Class C
    Implements I
    Public Sub Foo(Optional x As Object = 1R) Implements I.Foo
        Throw New NotImplementedException()
    End Sub
End Class")
        End Function

        <WorkItem(545860, "http://vstfdevdiv:8080/DevDiv2/DevDiv/_workitems/edit/545860")>
        <Fact, Trait(Traits.Feature, Traits.Features.CodeActionsImplementInterface)>
        Public Async Function TestLargeDecimal() As Task
            Await TestAsync(
"Option Strict On
Imports System
Interface I
    Sub Foo(Optional x As Decimal = 10000000000000000000D)
End Interface

Class C
    Implements [|I|]
End Class",
"Option Strict On
Imports System
Interface I
    Sub Foo(Optional x As Decimal = 10000000000000000000D)
End Interface

Class C
    Implements I
    Public Sub Foo(Optional x As Decimal = 10000000000000000000D) Implements I.Foo
        Throw New NotImplementedException()
    End Sub
End Class")
        End Function

        <WorkItem(545870, "http://vstfdevdiv:8080/DevDiv2/DevDiv/_workitems/edit/545870")>
        <Fact, Trait(Traits.Feature, Traits.Features.CodeActionsImplementInterface)>
        Public Async Function TestSurrogatePair1() As Task
            Await TestAsync(
"Interface I
    Sub Foo(Optional x As String = ""𪛖"")
End Interface
Class C
    Implements [|I|]
End Class",
"Imports System
Interface I
    Sub Foo(Optional x As String = ""𪛖"")
End Interface
Class C
    Implements I
    Public Sub Foo(Optional x As String = ""𪛖"") Implements I.Foo
        Throw New NotImplementedException()
    End Sub
End Class")
        End Function

        <WorkItem(545893, "http://vstfdevdiv:8080/DevDiv2/DevDiv/_workitems/edit/545893"), WorkItem(715013, "http://vstfdevdiv:8080/DevDiv2/DevDiv/_workitems/edit/715013")>
        <Fact(), Trait(Traits.Feature, Traits.Features.CodeActionsImplementInterface)>
        Public Async Function TestVBTab() As Task
            Await TestAsync(
"Imports System
Imports Microsoft.VisualBasic
Interface I
    Sub Foo(Optional x As String = vbTab)
End Interface

Class C
    Implements [|I|]
End Class",
"Imports System
Imports Microsoft.VisualBasic
Interface I
    Sub Foo(Optional x As String = vbTab)
End Interface

Class C
    Implements I
    Public Sub Foo(Optional x As String = vbTab) Implements I.Foo
        Throw New NotImplementedException()
    End Sub
End Class")
        End Function

        <WorkItem(545912, "http://vstfdevdiv:8080/DevDiv2/DevDiv/_workitems/edit/545912")>
        <Fact, Trait(Traits.Feature, Traits.Features.CodeActionsImplementInterface)>
        Public Async Function TestEscapeTypeParameter() As Task
            Await TestAsync(
"Interface I
    Sub Foo(Of [TO], TP, TQ)()
End Interface

Class C
    Implements [|I|]
End Class",
"Imports System
Interface I
    Sub Foo(Of [TO], TP, TQ)()
End Interface

Class C
    Implements I
    Public Sub Foo(Of [TO], TP, TQ)() Implements I.Foo
        Throw New NotImplementedException()
    End Sub
End Class")
        End Function

        <WorkItem(545892, "http://vstfdevdiv:8080/DevDiv2/DevDiv/_workitems/edit/545892")>
        <Fact, Trait(Traits.Feature, Traits.Features.CodeActionsImplementInterface)>
        Public Async Function TestLargeUnsignedLong() As Task
            Await TestAsync(
"Option Strict On
Imports System
Interface I
    Sub Foo(Optional x As ULong = 10000000000000000000UL)
End Interface

Class C
    Implements [|I|]
End Class",
"Option Strict On
Imports System
Interface I
    Sub Foo(Optional x As ULong = 10000000000000000000UL)
End Interface

Class C
    Implements I
    Public Sub Foo(Optional x As ULong = 10000000000000000000UL) Implements I.Foo
        Throw New NotImplementedException()
    End Sub
End Class")
        End Function

        <WorkItem(545865, "http://vstfdevdiv:8080/DevDiv2/DevDiv/_workitems/edit/545865")>
        <Fact, Trait(Traits.Feature, Traits.Features.CodeActionsImplementInterface)>
        Public Async Function TestSmallDecimalValues() As Task
            Dim markup =
<File>
Option Strict On

Interface I
    Sub F1(Optional x As Decimal = 1E-25D)
    Sub F2(Optional x As Decimal = 1E-26D)
    Sub F3(Optional x As Decimal = 1E-27D)
    Sub F4(Optional x As Decimal = 1E-28D)
    Sub F5(Optional x As Decimal = 1E-29D)
    Sub M1(Optional x As Decimal = 1.1E-25D)
    Sub M2(Optional x As Decimal = 1.1E-26D)
    Sub M3(Optional x As Decimal = 1.1E-27D)
    Sub M4(Optional x As Decimal = 1.1E-28D)
    Sub M5(Optional x As Decimal = 1.1E-29D)
    Sub S1(Optional x As Decimal = -1E-25D)
    Sub S2(Optional x As Decimal = -1E-26D)
    Sub S3(Optional x As Decimal = -1E-27D)
    Sub S4(Optional x As Decimal = -1E-28D)
    Sub S5(Optional x As Decimal = -1E-29D)
    Sub T1(Optional x As Decimal = -1.1E-25D)
    Sub T2(Optional x As Decimal = -1.1E-26D)
    Sub T3(Optional x As Decimal = -1.1E-27D)
    Sub T4(Optional x As Decimal = -1.1E-28D)
    Sub T5(Optional x As Decimal = -1.1E-29D)
End Interface

Class C
    Implements [|I|]
End Class
</File>

            Dim expected =
<File>
Option Strict On
Imports System

Interface I
    Sub F1(Optional x As Decimal = 1E-25D)
    Sub F2(Optional x As Decimal = 1E-26D)
    Sub F3(Optional x As Decimal = 1E-27D)
    Sub F4(Optional x As Decimal = 1E-28D)
    Sub F5(Optional x As Decimal = 1E-29D)
    Sub M1(Optional x As Decimal = 1.1E-25D)
    Sub M2(Optional x As Decimal = 1.1E-26D)
    Sub M3(Optional x As Decimal = 1.1E-27D)
    Sub M4(Optional x As Decimal = 1.1E-28D)
    Sub M5(Optional x As Decimal = 1.1E-29D)
    Sub S1(Optional x As Decimal = -1E-25D)
    Sub S2(Optional x As Decimal = -1E-26D)
    Sub S3(Optional x As Decimal = -1E-27D)
    Sub S4(Optional x As Decimal = -1E-28D)
    Sub S5(Optional x As Decimal = -1E-29D)
    Sub T1(Optional x As Decimal = -1.1E-25D)
    Sub T2(Optional x As Decimal = -1.1E-26D)
    Sub T3(Optional x As Decimal = -1.1E-27D)
    Sub T4(Optional x As Decimal = -1.1E-28D)
    Sub T5(Optional x As Decimal = -1.1E-29D)
End Interface

Class C
    Implements I

    Public Sub F1(Optional x As Decimal = 0.0000000000000000000000001D) Implements I.F1
        Throw New NotImplementedException()
    End Sub

    Public Sub F2(Optional x As Decimal = 0.00000000000000000000000001D) Implements I.F2
        Throw New NotImplementedException()
    End Sub

    Public Sub F3(Optional x As Decimal = 0.000000000000000000000000001D) Implements I.F3
        Throw New NotImplementedException()
    End Sub

    Public Sub F4(Optional x As Decimal = 0.0000000000000000000000000001D) Implements I.F4
        Throw New NotImplementedException()
    End Sub

    Public Sub F5(Optional x As Decimal = 0.0000000000000000000000000000D) Implements I.F5
        Throw New NotImplementedException()
    End Sub

    Public Sub M1(Optional x As Decimal = 0.00000000000000000000000011D) Implements I.M1
        Throw New NotImplementedException()
    End Sub

    Public Sub M2(Optional x As Decimal = 0.000000000000000000000000011D) Implements I.M2
        Throw New NotImplementedException()
    End Sub

    Public Sub M3(Optional x As Decimal = 0.0000000000000000000000000011D) Implements I.M3
        Throw New NotImplementedException()
    End Sub

    Public Sub M4(Optional x As Decimal = 0.0000000000000000000000000001D) Implements I.M4
        Throw New NotImplementedException()
    End Sub

    Public Sub M5(Optional x As Decimal = 0.0000000000000000000000000000D) Implements I.M5
        Throw New NotImplementedException()
    End Sub

    Public Sub S1(Optional x As Decimal = -0.0000000000000000000000001D) Implements I.S1
        Throw New NotImplementedException()
    End Sub

    Public Sub S2(Optional x As Decimal = -0.00000000000000000000000001D) Implements I.S2
        Throw New NotImplementedException()
    End Sub

    Public Sub S3(Optional x As Decimal = -0.000000000000000000000000001D) Implements I.S3
        Throw New NotImplementedException()
    End Sub

    Public Sub S4(Optional x As Decimal = -0.0000000000000000000000000001D) Implements I.S4
        Throw New NotImplementedException()
    End Sub

    Public Sub S5(Optional x As Decimal = 0.0000000000000000000000000000D) Implements I.S5
        Throw New NotImplementedException()
    End Sub

    Public Sub T1(Optional x As Decimal = -0.00000000000000000000000011D) Implements I.T1
        Throw New NotImplementedException()
    End Sub

    Public Sub T2(Optional x As Decimal = -0.000000000000000000000000011D) Implements I.T2
        Throw New NotImplementedException()
    End Sub

    Public Sub T3(Optional x As Decimal = -0.0000000000000000000000000011D) Implements I.T3
        Throw New NotImplementedException()
    End Sub

    Public Sub T4(Optional x As Decimal = -0.0000000000000000000000000001D) Implements I.T4
        Throw New NotImplementedException()
    End Sub

    Public Sub T5(Optional x As Decimal = 0.0000000000000000000000000000D) Implements I.T5
        Throw New NotImplementedException()
    End Sub
End Class
</File>

            Await TestAsync(markup, expected)
        End Function

        <WorkItem(544641, "http://vstfdevdiv:8080/DevDiv2/DevDiv/_workitems/edit/544641")>
        <Fact, Trait(Traits.Feature, Traits.Features.CodeActionsImplementInterface)>
        Public Async Function TestClassStatementTerminators1() As Task
            Await TestAsync(
"Imports System
Class C : Implements [|IServiceProvider|] : End Class",
"Imports System
Class C : Implements IServiceProvider
    Public Function GetService(serviceType As Type) As Object Implements IServiceProvider.GetService
        Throw New NotImplementedException()
    End Function
End Class")
        End Function

        <WorkItem(544641, "http://vstfdevdiv:8080/DevDiv2/DevDiv/_workitems/edit/544641")>
        <Fact, Trait(Traits.Feature, Traits.Features.CodeActionsImplementInterface)>
        Public Async Function TestClassStatementTerminators2() As Task
            Await TestAsync(
"Imports System
MustInherit Class D
    MustOverride Sub Foo()
End Class
Class C : Inherits D : Implements [|IServiceProvider|] : End Class",
"Imports System
MustInherit Class D
    MustOverride Sub Foo()
End Class
Class C : Inherits D : Implements IServiceProvider
    Public Function GetService(serviceType As Type) As Object Implements IServiceProvider.GetService
        Throw New NotImplementedException()
    End Function
End Class")
        End Function

        <WorkItem(544652, "http://vstfdevdiv:8080/DevDiv2/DevDiv/_workitems/edit/544652"), WorkItem(715013, "http://vstfdevdiv:8080/DevDiv2/DevDiv/_workitems/edit/715013")>
        <Fact(), Trait(Traits.Feature, Traits.Features.CodeActionsImplementInterface)>
        Public Async Function TestConvertNonprintableCharToString() As Task
            Await TestAsync(
"Imports System
Imports Microsoft.VisualBasic
Interface I
    Sub Foo(Optional x As Object = CStr(Chr(1)))
End Interface

Class C
    Implements [|I|] ' Implement 
End Class",
"Imports System
Imports Microsoft.VisualBasic
Interface I
    Sub Foo(Optional x As Object = CStr(Chr(1)))
End Interface

Class C
    Implements I ' Implement 
    Public Sub Foo(Optional x As Object = CStr(ChrW(1))) Implements I.Foo
        Throw New NotImplementedException()
    End Sub
End Class")
        End Function

        <WorkItem(545684, "http://vstfdevdiv:8080/DevDiv2/DevDiv/_workitems/edit/545684"), WorkItem(715013, "http://vstfdevdiv:8080/DevDiv2/DevDiv/_workitems/edit/715013")>
        <Fact(), Trait(Traits.Feature, Traits.Features.CodeActionsImplementInterface)>
        Public Async Function TestSimplifyModuleNameWhenPossible1() As Task
            Await TestAsync(
"Imports System
Imports Microsoft.VisualBasic
Interface I
    Sub Foo(Optional x As String = ChrW(1))
End Interface

Class C
    Implements [|I|]
End Class",
"Imports System
Imports Microsoft.VisualBasic
Interface I
    Sub Foo(Optional x As String = ChrW(1))
End Interface

Class C
    Implements I
    Public Sub Foo(Optional x As String = ChrW(1)) Implements I.Foo
        Throw New NotImplementedException()
    End Sub
End Class")
        End Function

        <WorkItem(545684, "http://vstfdevdiv:8080/DevDiv2/DevDiv/_workitems/edit/545684"), WorkItem(715013, "http://vstfdevdiv:8080/DevDiv2/DevDiv/_workitems/edit/715013")>
        <Fact(), Trait(Traits.Feature, Traits.Features.CodeActionsImplementInterface)>
        Public Async Function TestSimplifyModuleNameWhenPossible2() As Task
            Await TestAsync(
"Imports System
Imports Microsoft.VisualBasic
Interface I
    Sub Foo(Optional x As String = ChrW(1))
End Interface

Class C
    Implements [|I|]
    Public Sub ChrW(x As Integer)
    End Sub
End Class",
"Imports System
Imports Microsoft.VisualBasic
Interface I
    Sub Foo(Optional x As String = ChrW(1))
End Interface

Class C
    Implements I
    Public Sub ChrW(x As Integer)
    End Sub
    Public Sub Foo(Optional x As String = Strings.ChrW(1)) Implements I.Foo
        Throw New NotImplementedException()
    End Sub
End Class")
        End Function

        <WorkItem(544676, "http://vstfdevdiv:8080/DevDiv2/DevDiv/_workitems/edit/544676")>
        <Fact, Trait(Traits.Feature, Traits.Features.CodeActionsImplementInterface)>
        Public Async Function TestDoubleWideREM() As Task
            Await TestAsync(
"Interface I
    Sub ［ＲＥＭ］()
End Interface

Class C
    Implements [|I|]
End Class",
"Imports System
Interface I
    Sub ［ＲＥＭ］()
End Interface

Class C
    Implements I
    Public Sub [ＲＥＭ]() Implements I.[ＲＥＭ]
        Throw New NotImplementedException()
    End Sub
End Class")
        End Function

        <WorkItem(545917, "http://vstfdevdiv:8080/DevDiv2/DevDiv/_workitems/edit/545917")>
        <Fact, Trait(Traits.Feature, Traits.Features.CodeActionsImplementInterface)>
        Public Async Function TestDoubleWideREM2() As Task
            Await TestAsync(
"Interface I
    Sub Foo(Of ［ＲＥＭ］)()
End Interface
Class C
    Implements [|I|]
End Class",
"Imports System
Interface I
    Sub Foo(Of ［ＲＥＭ］)()
End Interface
Class C
    Implements I

    Public Sub Foo(Of [ＲＥＭ])() Implements I.Foo
        Throw New NotImplementedException()
    End Sub
End Class")
        End Function

        <WorkItem(545953, "http://vstfdevdiv:8080/DevDiv2/DevDiv/_workitems/edit/545953")>
        <Fact, Trait(Traits.Feature, Traits.Features.CodeActionsImplementInterface)>
        Public Async Function TestGenericEnumWithRenamedTypeParameters1() As Task
            Await TestAsync(
"Option Strict On
Class C(Of T)
    Enum E
        X
    End Enum
End Class
Interface I
    Sub Foo(Of M)(Optional x As C(Of M()).E = C(Of M()).E.X)
End Interface
Class C
    Implements [|I|] ' Implement 
End Class",
"Option Strict On
Imports System
Class C(Of T)
    Enum E
        X
    End Enum
End Class
Interface I
    Sub Foo(Of M)(Optional x As C(Of M()).E = C(Of M()).E.X)
End Interface
Class C
    Implements I ' Implement 
    Public Sub Foo(Of M)(Optional x As C(Of M()).E = C(Of M()).E.X) Implements I.Foo
        Throw New NotImplementedException()
    End Sub
End Class")
        End Function

        <WorkItem(545953, "http://vstfdevdiv:8080/DevDiv2/DevDiv/_workitems/edit/545953")>
        <Fact, Trait(Traits.Feature, Traits.Features.CodeActionsImplementInterface)>
        Public Async Function TestGenericEnumWithRenamedTypeParameters2() As Task
            Await TestAsync(
"Option Strict On
Class C(Of T)
    Enum E
        X
    End Enum
End Class
Interface I
    Sub Foo(Of T)(Optional x As C(Of T()).E = C(Of T()).E.X)
End Interface
Class C
    Implements [|I|]
End Class",
"Option Strict On
Imports System
Class C(Of T)
    Enum E
        X
    End Enum
End Class
Interface I
    Sub Foo(Of T)(Optional x As C(Of T()).E = C(Of T()).E.X)
End Interface
Class C
    Implements I
    Public Sub Foo(Of T)(Optional x As C(Of T()).E = C(Of T()).E.X) Implements I.Foo
        Throw New NotImplementedException()
    End Sub
End Class")
        End Function

        <WorkItem(546197, "http://vstfdevdiv:8080/DevDiv2/DevDiv/_workitems/edit/546197")>
        <Fact, Trait(Traits.Feature, Traits.Features.CodeActionsImplementInterface)>
        Public Async Function TestDoubleQuoteChar() As Task
            Await TestAsync(
"Imports System
Interface I
    Sub Foo(Optional x As Object = """"""""c)
End Interface

Class C
    Implements [|I|]
End Class",
"Imports System
Interface I
    Sub Foo(Optional x As Object = """"""""c)
End Interface

Class C
    Implements I
    Public Sub Foo(Optional x As Object = """"""""c) Implements I.Foo
        Throw New NotImplementedException()
    End Sub
End Class")
        End Function

        <WorkItem(530165, "http://vstfdevdiv:8080/DevDiv2/DevDiv/_workitems/edit/530165")>
        <Fact, Trait(Traits.Feature, Traits.Features.CodeActionsImplementInterface)>
        Public Async Function TestGenerateIntoAppropriatePartial() As Task
            Await TestAsync(
"Interface I

    Sub M()

End Interface

Class C

End Class

Partial Class C
    Implements [|I|]
End Class",
"Imports System
Interface I

    Sub M()

End Interface

Class C

End Class

Partial Class C
    Implements I
    Public Sub M() Implements I.M
        Throw New NotImplementedException()
    End Sub
End Class")
        End Function

        <WorkItem(546325, "http://vstfdevdiv:8080/DevDiv2/DevDiv/_workitems/edit/546325")>
        <Fact, Trait(Traits.Feature, Traits.Features.CodeActionsImplementInterface)>
        Public Async Function TestAttributes() As Task
            Await TestAsync(
"Imports System.Runtime.InteropServices

Interface I
    Function Foo(<MarshalAs(UnmanagedType.U1)> x As Boolean) As <MarshalAs(UnmanagedType.U1)> Boolean
End Interface

Class C
    Implements [|I|]
End Class",
"Imports System
Imports System.Runtime.InteropServices

Interface I
    Function Foo(<MarshalAs(UnmanagedType.U1)> x As Boolean) As <MarshalAs(UnmanagedType.U1)> Boolean
End Interface

Class C
    Implements I
    Public Function Foo(<MarshalAs(UnmanagedType.U1)>
    x As Boolean) As <MarshalAs(UnmanagedType.U1)>
    Boolean Implements I.Foo
        Throw New NotImplementedException()
    End Function
End Class")
        End Function

        <WorkItem(530564, "http://vstfdevdiv:8080/DevDiv2/DevDiv/_workitems/edit/530564")>
        <Fact, Trait(Traits.Feature, Traits.Features.CodeActionsImplementInterface)>
        Public Async Function TestShortenedDecimal() As Task
            Await TestAsync(
"Option Strict On
Interface I
    Sub Foo(Optional x As Decimal = 1000000000000000000D)
End Interface
Class C
    Implements [|I|] ' Implement 
End Class",
"Option Strict On
Imports System
Interface I
    Sub Foo(Optional x As Decimal = 1000000000000000000D)
End Interface
Class C
    Implements I ' Implement 
    Public Sub Foo(Optional x As Decimal = 1000000000000000000) Implements I.Foo
        Throw New NotImplementedException()
    End Sub
End Class")
        End Function

        <WorkItem(530713, "http://vstfdevdiv:8080/DevDiv2/DevDiv/_workitems/edit/530713")>
        <Fact, Trait(Traits.Feature, Traits.Features.CodeActionsImplementInterface)>
        Public Async Function TestImplementAbstractly2() As Task
            Await TestAsync(
"Interface I
    Property Foo() As Integer
End Interface

MustInherit Class C
    Implements [|I|] ' Implement interface abstractly 
End Class",
"Imports System
Interface I
    Property Foo() As Integer
End Interface

MustInherit Class C
    Implements I ' Implement interface abstractly 
    Public Property Foo As Integer Implements I.Foo
        Get
            Throw New NotImplementedException()
        End Get
        Set(value As Integer)
            Throw New NotImplementedException()
        End Set
    End Property
End Class")
        End Function

        <WorkItem(916114, "http://vstfdevdiv:8080/DevDiv2/DevDiv/_workitems/edit/916114")>
        <Fact, Trait(Traits.Feature, Traits.Features.CodeActionsImplementInterface)>
        Public Async Function TestOptionalNullableStructParameter() As Task
            Await TestAsync(
"Interface I
    ReadOnly Property g(Optional x As S? = Nothing)
End Interface
Class c
    Implements [|I|]
End Class
Structure S
End Structure",
"Imports System
Interface I
    ReadOnly Property g(Optional x As S? = Nothing)
End Interface
Class c
    Implements I
    Public ReadOnly Property g(Optional x As S? = Nothing) As Object Implements I.g
        Get
            Throw New NotImplementedException()
        End Get
    End Property
End Class
Structure S
End Structure")
        End Function

        <WorkItem(916114, "http://vstfdevdiv:8080/DevDiv2/DevDiv/_workitems/edit/916114")>
        <Fact, Trait(Traits.Feature, Traits.Features.CodeActionsImplementInterface)>
        Public Async Function TestOptionalNullableLongParameter() As Task
            Await TestAsync(
"Interface I
    ReadOnly Property g(Optional x As Long? = Nothing, Optional y As Long? = 5)
End Interface
Class c
    Implements [|I|]
End Class",
"Imports System
Interface I
    ReadOnly Property g(Optional x As Long? = Nothing, Optional y As Long? = 5)
End Interface
Class c
    Implements I
    Public ReadOnly Property g(Optional x As Long? = Nothing, Optional y As Long? = 5) As Object Implements I.g
        Get
            Throw New NotImplementedException()
        End Get
    End Property
End Class")
        End Function

        <WorkItem(530345, "http://vstfdevdiv:8080/DevDiv2/DevDiv/_workitems/edit/530345")>
        <Fact, Trait(Traits.Feature, Traits.Features.CodeActionsImplementInterface)>
        Public Async Function TestAttributeFormattingInNonStatementContext() As Task
            Await TestAsync(
<Text>Imports System.Runtime.InteropServices

Interface I
    Function Foo(&lt;MarshalAs(UnmanagedType.U1)&gt; x As Boolean) As &lt;MarshalAs(UnmanagedType.U1)&gt; Boolean
End Interface

Class C
    Implements [|I|] ' Implement
End Class
</Text>.Value.Replace(vbLf, vbCrLf),
<Text>Imports System
Imports System.Runtime.InteropServices

Interface I
    Function Foo(&lt;MarshalAs(UnmanagedType.U1)&gt; x As Boolean) As &lt;MarshalAs(UnmanagedType.U1)&gt; Boolean
End Interface

Class C
    Implements I ' Implement

    Public Function Foo(&lt;MarshalAs(UnmanagedType.U1)&gt; x As Boolean) As &lt;MarshalAs(UnmanagedType.U1)&gt; Boolean Implements I.Foo
        Throw New NotImplementedException()
    End Function
End Class
</Text>.Value.Replace(vbLf, vbCrLf),
compareTokens:=False)
        End Function

        <WorkItem(546779, "http://vstfdevdiv:8080/DevDiv2/DevDiv/_workitems/edit/546779")>
        <Fact, Trait(Traits.Feature, Traits.Features.CodeActionsImplementInterface)>
        Public Async Function TestPropertyReturnTypeAttributes() As Task
            Await TestAsync(
"Imports System.Runtime.InteropServices

Interface I
    Property P(<MarshalAs(UnmanagedType.I4)> x As Integer) As <MarshalAs(UnmanagedType.I4)> Integer
End Interface

Class C
    Implements [|I|]
End Class",
"Imports System
Imports System.Runtime.InteropServices

Interface I
    Property P(<MarshalAs(UnmanagedType.I4)> x As Integer) As <MarshalAs(UnmanagedType.I4)> Integer
End Interface

Class C
    Implements I
    Public Property P(<MarshalAs(UnmanagedType.I4)> x As Integer) As <MarshalAs(UnmanagedType.I4)> Integer Implements I.P
        Get
            Throw New NotImplementedException()
        End Get
        Set(value As Integer)
            Throw New NotImplementedException()
        End Set
    End Property
End Class")
        End Function

        <WorkItem(847464, "http://vstfdevdiv:8080/DevDiv2/DevDiv/_workitems/edit/847464")>
        <Fact, Trait(Traits.Feature, Traits.Features.CodeActionsImplementInterface)>
        Public Async Function TestImplementInterfaceForPartialType() As Task
            Await TestAsync(
"Public Interface I
    Sub Foo()
End Interface
Partial Class C
End Class
Partial Class C
    Implements [|I|]
End Class",
"Imports System
Public Interface I
    Sub Foo()
End Interface
Partial Class C
End Class
Partial Class C
    Implements I
    Public Sub Foo() Implements I.Foo
        Throw New NotImplementedException()
    End Sub
End Class")
        End Function

        <WorkItem(617698, "http://vstfdevdiv:8080/DevDiv2/DevDiv/_workitems/edit/617698")>
        <Fact, Trait(Traits.Feature, Traits.Features.CodeActionsImplementInterface)>
        Public Async Function TestBugfix_617698_RecursiveSimplificationOfQualifiedName() As Task
            Await TestAsync(
<Text>Interface A(Of B)
    Sub M()
    Interface C(Of D)
        Inherits A(Of C(Of D))
        Interface E
            Inherits C(Of E)
            Class D
                Implements [|E|]
            End Class
        End Interface
    End Interface
End Interface
</Text>.Value.Replace(vbLf, vbCrLf),
<Text>Imports System

Interface A(Of B)
    Sub M()
    Interface C(Of D)
        Inherits A(Of C(Of D))
        Interface E
            Inherits C(Of E)
            Class D
                Implements E

                Public Sub M() Implements A(Of A(Of A(Of A(Of B).C(Of D)).C(Of A(Of B).C(Of D).E)).C(Of A(Of A(Of B).C(Of D)).C(Of A(Of B).C(Of D).E).E)).M
                    Throw New NotImplementedException()
                End Sub
            End Class
        End Interface
    End Interface
End Interface
</Text>.Value.Replace(vbLf, vbCrLf),
compareTokens:=False)
        End Function

        <Fact, Trait(Traits.Feature, Traits.Features.CodeActionsImplementInterface)>
        Public Async Function TestImplementInterfaceForIDisposable() As Task
            Await TestAsync(
<Text>Imports System
Class Program
    Implements [|IDisposable|]

End Class
</Text>.Value.Replace(vbLf, vbCrLf),
$"Imports System
Class Program
    Implements IDisposable
{DisposePattern("Overridable ")}

End Class
",
index:=1,
compareTokens:=False)
        End Function

        <Fact, Trait(Traits.Feature, Traits.Features.CodeActionsImplementInterface)>
        Public Async Function TestImplementInterfaceForIDisposableNonApplicable1() As Task
            Await TestAsync(
<Text>Imports System
Class Program
    Implements [|IDisposable|]

    Private DisposedValue As Boolean

End Class
</Text>.Value.Replace(vbLf, vbCrLf),
<Text>Imports System
Class Program
    Implements IDisposable

    Private DisposedValue As Boolean

    Public Sub Dispose() Implements IDisposable.Dispose
        Throw New NotImplementedException()
    End Sub
End Class
</Text>.Value.Replace(vbLf, vbCrLf),
compareTokens:=False)
        End Function

        <Fact, Trait(Traits.Feature, Traits.Features.CodeActionsImplementInterface)>
        Public Async Function TestImplementInterfaceForIDisposableNonApplicable2() As Task
            Await TestAsync(
"Imports System
Class Program
    Implements [|IDisposable|]

    Public Sub Dispose(flag As Boolean)
    End Sub
End Class",
"Imports System
Class Program
    Implements IDisposable

    Public Sub Dispose(flag As Boolean)
    End Sub

    Public Sub Dispose() Implements IDisposable.Dispose
        Throw New NotImplementedException()
    End Sub
End Class")
        End Function

        <Fact, Trait(Traits.Feature, Traits.Features.CodeActionsImplementInterface)>
        Public Async Function TestImplementInterfaceForIDisposableWithSealedClass() As Task
            Await TestAsync(
<Text>Imports System
Public NotInheritable Class Program
    Implements [|IDisposable|]

End Class
</Text>.Value.Replace(vbLf, vbCrLf),
$"Imports System
Public NotInheritable Class Program
    Implements IDisposable
{DisposePattern("")}

End Class
",
index:=1,
compareTokens:=False)
        End Function

        <WorkItem(939123, "http://vstfdevdiv:8080/DevDiv2/DevDiv/_workitems/edit/939123")>
        <Fact, Trait(Traits.Feature, Traits.Features.CodeActionsImplementInterface)>
        Public Async Function TestNoComAliasNameAttributeOnMethodParameters() As Task
            Await TestAsync(
"Imports System.Runtime.InteropServices

Interface I
    Function F(<ComAliasName(""pAlias"")> p As Long) As Integer
End Interface

MustInherit Class C
    Implements [|I|]
End Class",
"Imports System
Imports System.Runtime.InteropServices

Interface I
    Function F(<ComAliasName(""pAlias"")> p As Long) As Integer
End Interface

MustInherit Class C
    Implements I

    Public Function F(p As Long) As Integer Implements I.F
        Throw New NotImplementedException()
    End Function
End Class")
        End Function

        <WorkItem(939123, "http://vstfdevdiv:8080/DevDiv2/DevDiv/_workitems/edit/939123")>
        <Fact, Trait(Traits.Feature, Traits.Features.CodeActionsImplementInterface)>
        Public Async Function TestNoComAliasNameAttributeOnMethodReturnType() As Task
            Await TestAsync(
"Imports System.Runtime.InteropServices

Interface I
    Function F(<ComAliasName(""pAlias1"")> p As Long) As <ComAliasName(""pAlias2"")> Integer
End Interface

MustInherit Class C
    Implements [|I|]
End Class",
"Imports System
Imports System.Runtime.InteropServices

Interface I
    Function F(<ComAliasName(""pAlias1"")> p As Long) As <ComAliasName(""pAlias2"")> Integer
End Interface

MustInherit Class C
    Implements I

    Public Function F(p As Long) As Integer Implements I.F
        Throw New NotImplementedException()
    End Function
End Class")
        End Function

        <WorkItem(939123, "http://vstfdevdiv:8080/DevDiv2/DevDiv/_workitems/edit/939123")>
        <Fact, Trait(Traits.Feature, Traits.Features.CodeActionsImplementInterface)>
        Public Async Function TestNoComAliasNameAttributeOnPropertyParameters() As Task
            Await TestAsync(
"Imports System.Runtime.InteropServices

Interface I
    Default Property Prop(<ComAliasName(""pAlias"")> p As Long) As Integer
End Interface

Class C
    Implements [|I|]
End Class",
"Imports System
Imports System.Runtime.InteropServices

Interface I
    Default Property Prop(<ComAliasName(""pAlias"")> p As Long) As Integer
End Interface

Class C
    Implements I

    Default Public Property Prop(p As Long) As Integer Implements I.Prop

        Get
            Throw New NotImplementedException()
        End Get

        Set(value As Integer)
            Throw New NotImplementedException()
        End Set

    End Property
End Class")
        End Function

        <WorkItem(529920, "http://vstfdevdiv:8080/DevDiv2/DevDiv/_workitems/edit/529920")>
        <Fact, Trait(Traits.Feature, Traits.Features.CodeActionsImplementInterface)>
        Public Async Function TestNewLineBeforeDirective() As Task
            Await TestAsync(
"Imports System
Class C 
    Implements [|IServiceProvider|]
#Disable Warning",
"Imports System
Class C 
    Implements IServiceProvider

    Public Function GetService(serviceType As Type) As Object Implements IServiceProvider.GetService
        Throw New NotImplementedException()
    End Function
End Class
#Disable Warning", compareTokens:=False)
        End Function

        <WorkItem(529947, "http://vstfdevdiv:8080/DevDiv2/DevDiv/_workitems/edit/529947")>
        <Fact, Trait(Traits.Feature, Traits.Features.CodeActionsImplementInterface)>
        Public Async Function TestCommentAfterInterfaceList1() As Task
            Await TestAsync(
"Imports System
Class C 
    Implements [|IServiceProvider|] REM Comment",
"Imports System
Class C 
    Implements IServiceProvider REM Comment

    Public Function GetService(serviceType As Type) As Object Implements IServiceProvider.GetService
        Throw New NotImplementedException()
    End Function
End Class
", compareTokens:=False)
        End Function

        <WorkItem(529947, "http://vstfdevdiv:8080/DevDiv2/DevDiv/_workitems/edit/529947")>
        <Fact, Trait(Traits.Feature, Traits.Features.CodeActionsImplementInterface)>
        Public Async Function TestCommentAfterInterfaceList2() As Task
            Await TestAsync(
"Imports System
Class C
    Implements [|IServiceProvider|]
REM Comment",
"Imports System
Class C
    Implements IServiceProvider

    Public Function GetService(serviceType As Type) As Object Implements IServiceProvider.GetService
        Throw New NotImplementedException()
    End Function
End Class
REM Comment", compareTokens:=False)
        End Function

        <WorkItem(994456, "http://vstfdevdiv:8080/DevDiv2/DevDiv/_workitems/edit/994456")>
        <WorkItem(958699, "http://vstfdevdiv:8080/DevDiv2/DevDiv/_workitems/edit/958699")>
        <Fact, Trait(Traits.Feature, Traits.Features.CodeActionsImplementInterface)>
        Public Async Function TestImplementIDisposable_NoDisposePattern() As Task
            Await TestAsync(
"Imports System
Class C : Implements [|IDisposable|]",
"Imports System
Class C : Implements IDisposable

    Public Sub Dispose() Implements IDisposable.Dispose
        Throw New NotImplementedException()
    End Sub
End Class
", index:=0, compareTokens:=False)
        End Function

        <WorkItem(994456, "http://vstfdevdiv:8080/DevDiv2/DevDiv/_workitems/edit/994456")>
        <WorkItem(958699, "http://vstfdevdiv:8080/DevDiv2/DevDiv/_workitems/edit/958699")>
        <Fact, Trait(Traits.Feature, Traits.Features.CodeActionsImplementInterface)>
        Public Async Function TestImplementIDisposable1_DisposePattern() As Task
            Await TestAsync(
"Imports System
Class C : Implements [|IDisposable|]",
$"Imports System
Class C : Implements IDisposable
{DisposePattern("Overridable ")}
End Class
", index:=1, compareTokens:=False)
        End Function

        <WorkItem(994456, "http://vstfdevdiv:8080/DevDiv2/DevDiv/_workitems/edit/994456")>
        <WorkItem(958699, "http://vstfdevdiv:8080/DevDiv2/DevDiv/_workitems/edit/958699")>
        <Fact, Trait(Traits.Feature, Traits.Features.CodeActionsImplementInterface)>
        Public Async Function TestImplementIDisposableAbstractly_NoDisposePattern() As Task
            Await TestAsync(
"Imports System
MustInherit Class C : Implements [|IDisposable|]",
"Imports System
MustInherit Class C : Implements IDisposable

    Public MustOverride Sub Dispose() Implements IDisposable.Dispose
End Class
", index:=2, compareTokens:=False)
        End Function

        <WorkItem(994456, "http://vstfdevdiv:8080/DevDiv2/DevDiv/_workitems/edit/994456")>
        <WorkItem(958699, "http://vstfdevdiv:8080/DevDiv2/DevDiv/_workitems/edit/958699")>
        <Fact, Trait(Traits.Feature, Traits.Features.CodeActionsImplementInterface)>
        Public Async Function TestImplementIDisposableThroughMember_NoDisposePattern() As Task
            Await TestAsync(
"Imports System
Class C : Implements [|IDisposable|]
    Dim foo As IDisposable
End Class",
"Imports System
Class C : Implements IDisposable
    Dim foo As IDisposable

    Public Sub Dispose() Implements IDisposable.Dispose
        foo.Dispose()
    End Sub
End Class", index:=2, compareTokens:=False)
        End Function

        <WorkItem(941469, "http://vstfdevdiv:8080/DevDiv2/DevDiv/_workitems/edit/941469")>
        <Fact, Trait(Traits.Feature, Traits.Features.CodeActionsImplementInterface)>
        Public Async Function TestImplementIDisposable2() As Task
            Await TestAsync(
"Imports System
Class C : Implements [|System.IDisposable|]
    Class IDisposable
    End Class
End Class",
$"Imports System
Class C : Implements System.IDisposable
    Class IDisposable
    End Class
{DisposePattern("Overridable ", simplifySystem:=False)}
End Class", index:=1, compareTokens:=False)
        End Function

        <WorkItem(958699, "http://vstfdevdiv:8080/DevDiv2/DevDiv/_workitems/edit/958699")>
        <Fact, Trait(Traits.Feature, Traits.Features.CodeActionsImplementInterface)>
        Public Async Function TestImplementIDisposable_NoNamespaceImportForSystem() As Task
            Await TestAsync(
"Class C : Implements [|System.IDisposable|]
",
$"Class C : Implements System.IDisposable
{DisposePattern("Overridable ", simplifySystem:=False)}
End Class
", index:=1, compareTokens:=False)
        End Function

        <WorkItem(951968, "http://vstfdevdiv:8080/DevDiv2/DevDiv/_workitems/edit/951968")>
        <Fact, Trait(Traits.Feature, Traits.Features.CodeActionsImplementInterface)>
        Public Async Function TestImplementIDisposableViaBaseInterface_NoDisposePattern() As Task
            Await TestAsync(
"Imports System
Interface I : Inherits IDisposable
    Sub F()
End Interface
Class C : Implements [|I|]
End Class",
"Imports System
Interface I : Inherits IDisposable
    Sub F()
End Interface
Class C : Implements I

    Public Sub F() Implements I.F
        Throw New NotImplementedException()
    End Sub

    Public Sub Dispose() Implements IDisposable.Dispose
        Throw New NotImplementedException()
    End Sub
End Class", index:=0)
        End Function

        <WorkItem(951968, "http://vstfdevdiv:8080/DevDiv2/DevDiv/_workitems/edit/951968")>
        <Fact, Trait(Traits.Feature, Traits.Features.CodeActionsImplementInterface)>
        Public Async Function TestImplementIDisposableViaBaseInterface() As Task
            Await TestAsync(
"Imports System
Interface I : Inherits IDisposable
    Sub F()
End Interface
Class C : Implements [|I|]
End Class",
$"Imports System
Interface I : Inherits IDisposable
    Sub F()
End Interface
Class C : Implements I

    Public Sub F() Implements I.F
        Throw New NotImplementedException()
    End Sub
{DisposePattern("Overridable ")}
End Class", index:=1, compareTokens:=False)
        End Function

        <WorkItem(951968, "http://vstfdevdiv:8080/DevDiv2/DevDiv/_workitems/edit/951968")>
        <Fact, Trait(Traits.Feature, Traits.Features.CodeActionsImplementInterface)>
        Public Async Function TestDontImplementDisposePatternForLocallyDefinedIDisposable() As Task
            Await TestAsync(
"Namespace System
    Interface IDisposable
        Sub Dispose
    End Interface

    Class C : Implements [|IDisposable|]
End Namespace",
"Namespace System
    Interface IDisposable
        Sub Dispose
    End Interface

    Class C : Implements IDisposable

        Public Sub Dispose() Implements IDisposable.Dispose
            Throw New NotImplementedException()
        End Sub
    End Class
End Namespace", compareTokens:=False)
        End Function

        <Fact, Trait(Traits.Feature, Traits.Features.CodeActionsImplementInterface)>
        Public Async Function TestDontImplementDisposePatternForStructures() As Task
            Await TestAsync(
"Imports System
Structure S : Implements [|IDisposable|]",
"Imports System
Structure S : Implements IDisposable

    Public Sub Dispose() Implements IDisposable.Dispose
        Throw New NotImplementedException()
    End Sub
End Structure
", compareTokens:=False)
        End Function

        <WorkItem(994328, "http://vstfdevdiv:8080/DevDiv2/DevDiv/_workitems/edit/994328")>
        <Fact, Trait(Traits.Feature, Traits.Features.CodeActionsImplementInterface)>
        Public Async Function TestDisposePatternWhenAdditionalImportsAreIntroduced1() As Task
            Await TestAsync(
"Interface I(Of T, U As T) : Inherits System.IDisposable, System.IEquatable(Of Integer)
    Function M(a As System.Collections.Generic.Dictionary(Of T, System.Collections.Generic.List(Of U)), b As T, c As U) As System.Collections.Generic.List(Of U)
    Function M(Of TT, UU As TT)(a As System.Collections.Generic.Dictionary(Of TT, System.Collections.Generic.List(Of UU)), b As TT, c As UU) As System.Collections.Generic.List(Of UU)
End Interface

Class _
    C
    Implements [|I(Of System.Exception, System.AggregateException)|]
End Class

Partial Class C
    Implements IDisposable
End Class",
$"Imports System
Imports System.Collections.Generic

Interface I(Of T, U As T) : Inherits System.IDisposable, System.IEquatable(Of Integer)
    Function M(a As System.Collections.Generic.Dictionary(Of T, System.Collections.Generic.List(Of U)), b As T, c As U) As System.Collections.Generic.List(Of U)
    Function M(Of TT, UU As TT)(a As System.Collections.Generic.Dictionary(Of TT, System.Collections.Generic.List(Of UU)), b As TT, c As UU) As System.Collections.Generic.List(Of UU)
End Interface

Class _
    C
    Implements I(Of System.Exception, System.AggregateException)

    Public Function M(a As Dictionary(Of Exception, List(Of AggregateException)), b As Exception, c As AggregateException) As List(Of AggregateException) Implements I(Of Exception, AggregateException).M
        Throw New NotImplementedException()
    End Function

    Public Function M(Of TT, UU As TT)(a As Dictionary(Of TT, List(Of UU)), b As TT, c As UU) As List(Of UU) Implements I(Of Exception, AggregateException).M
        Throw New NotImplementedException()
    End Function

    Public Function Equals(other As Integer) As Boolean Implements IEquatable(Of Integer).Equals
        Throw New NotImplementedException()
    End Function

#Region ""IDisposable Support""
    Private disposedValue As Boolean ' To detect redundant calls

    ' IDisposable
    Protected Overridable Sub Dispose(disposing As Boolean)
        If Not disposedValue Then
            If disposing Then
                ' TODO: dispose managed state (managed objects).
            End If

            ' TODO: free unmanaged resources (unmanaged objects) and override Finalize() below.
            ' TODO: set large fields to null.
        End If
        disposedValue = True
    End Sub

    ' TODO: override Finalize() only if Dispose(disposing As Boolean) above has code to free unmanaged resources.
    'Protected Overrides Sub Finalize()
    '    ' Do not change this code.  Put cleanup code in Dispose(disposing As Boolean) above.
    '    Dispose(False)
    '    MyBase.Finalize()
    'End Sub

    ' This code added by Visual Basic to correctly implement the disposable pattern.
    Public Sub Dispose() Implements IDisposable.Dispose
        ' Do not change this code.  Put cleanup code in Dispose(disposing As Boolean) above.
        Dispose(True)
        ' TODO: uncomment the following line if Finalize() is overridden above.
        ' GC.SuppressFinalize(Me)
    End Sub
#End Region
End Class

Partial Class C
    Implements IDisposable
End Class",
 index:=1, compareTokens:=False)
        End Function

        <WorkItem(994328, "http://vstfdevdiv:8080/DevDiv2/DevDiv/_workitems/edit/994328")>
        <Fact, Trait(Traits.Feature, Traits.Features.CodeActionsImplementInterface)>
        Public Async Function TestDisposePatternWhenAdditionalImportsAreIntroduced2() As Task
            Await TestAsync(
"Class C
End Class

Partial Class C
    Implements [|I(Of System.Exception, System.AggregateException)|]
    Implements IDisposable
End Class

Interface I(Of T, U As T) : Inherits System.IDisposable, System.IEquatable(Of Integer)
    Function M(a As System.Collections.Generic.Dictionary(Of T, System.Collections.Generic.List(Of U)), b As T, c As U) As System.Collections.Generic.List(Of U)
    Function M(Of TT, UU As TT)(a As System.Collections.Generic.Dictionary(Of TT, System.Collections.Generic.List(Of UU)), b As TT, c As UU) As System.Collections.Generic.List(Of UU)
End Interface",
$"Imports System
Imports System.Collections.Generic

Class C
End Class

Partial Class C
    Implements I(Of System.Exception, System.AggregateException)
    Implements IDisposable

    Public Function M(a As Dictionary(Of Exception, List(Of AggregateException)), b As Exception, c As AggregateException) As List(Of AggregateException) Implements I(Of Exception, AggregateException).M
        Throw New NotImplementedException()
    End Function

    Public Function M(Of TT, UU As TT)(a As Dictionary(Of TT, List(Of UU)), b As TT, c As UU) As List(Of UU) Implements I(Of Exception, AggregateException).M
        Throw New NotImplementedException()
    End Function

    Public Function Equals(other As Integer) As Boolean Implements IEquatable(Of Integer).Equals
        Throw New NotImplementedException()
    End Function

#Region ""IDisposable Support""
    Private disposedValue As Boolean ' To detect redundant calls

    ' IDisposable
    Protected Overridable Sub Dispose(disposing As Boolean)
        If Not disposedValue Then
            If disposing Then
                ' TODO: dispose managed state (managed objects).
            End If

            ' TODO: free unmanaged resources (unmanaged objects) and override Finalize() below.
            ' TODO: set large fields to null.
        End If
        disposedValue = True
    End Sub

    ' TODO: override Finalize() only if Dispose(disposing As Boolean) above has code to free unmanaged resources.
    'Protected Overrides Sub Finalize()
    '    ' Do not change this code.  Put cleanup code in Dispose(disposing As Boolean) above.
    '    Dispose(False)
    '    MyBase.Finalize()
    'End Sub

    ' This code added by Visual Basic to correctly implement the disposable pattern.
    Public Sub Dispose() Implements IDisposable.Dispose
        ' Do not change this code.  Put cleanup code in Dispose(disposing As Boolean) above.
        Dispose(True)
        ' TODO: uncomment the following line if Finalize() is overridden above.
        ' GC.SuppressFinalize(Me)
    End Sub
#End Region
End Class

Interface I(Of T, U As T) : Inherits System.IDisposable, System.IEquatable(Of Integer)
    Function M(a As System.Collections.Generic.Dictionary(Of T, System.Collections.Generic.List(Of U)), b As T, c As U) As System.Collections.Generic.List(Of U)
    Function M(Of TT, UU As TT)(a As System.Collections.Generic.Dictionary(Of TT, System.Collections.Generic.List(Of UU)), b As TT, c As UU) As System.Collections.Generic.List(Of UU)
End Interface",
 index:=1)
        End Function

        Private Shared Function DisposePattern(disposeMethodModifiers As String, Optional simplifySystem As Boolean = True) As String
            Dim code = $"
#Region ""IDisposable Support""
    Private disposedValue As Boolean ' {FeaturesResources.To_detect_redundant_calls}

    ' IDisposable
    Protected {disposeMethodModifiers}Sub Dispose(disposing As Boolean)
        If Not disposedValue Then
            If disposing Then
                ' {FeaturesResources.TODO_colon_dispose_managed_state_managed_objects}
            End If

            ' {VBFeaturesResources.TODO_colon_free_unmanaged_resources_unmanaged_objects_and_override_Finalize_below}
            ' {FeaturesResources.TODO_colon_set_large_fields_to_null}
        End If
        disposedValue = True
    End Sub

    ' {VBFeaturesResources.TODO_colon_override_Finalize_only_if_Dispose_disposing_As_Boolean_above_has_code_to_free_unmanaged_resources}
    'Protected Overrides Sub Finalize()
    '    ' {VBFeaturesResources.Do_not_change_this_code_Put_cleanup_code_in_Dispose_disposing_As_Boolean_above}
    '    Dispose(False)
    '    MyBase.Finalize()
    'End Sub

    ' {VBFeaturesResources.This_code_added_by_Visual_Basic_to_correctly_implement_the_disposable_pattern}
    Public Sub Dispose() Implements System.IDisposable.Dispose
        ' {VBFeaturesResources.Do_not_change_this_code_Put_cleanup_code_in_Dispose_disposing_As_Boolean_above}
        Dispose(True)
        ' {VBFeaturesResources.TODO_colon_uncomment_the_following_line_if_Finalize_is_overridden_above}
        ' GC.SuppressFinalize(Me)
    End Sub
#End Region"

            ' some tests count on "System." being simplified out
            If simplifySystem Then
                code = code.Replace("System.IDisposable.Dispose", "IDisposable.Dispose")
            End If

            Return code
        End Function

        <WorkItem(1132014, "http://vstfdevdiv:8080/DevDiv2/DevDiv/_workitems/edit/1132014")>
        <Fact, Trait(Traits.Feature, Traits.Features.CodeActionsImplementInterface)>
        Public Async Function TestInaccessibleAttributes() As Task
            Await TestAsync(
"Imports System

Public Class Foo
    Implements [|Holder.SomeInterface|]
End Class

Public Class Holder
	Public Interface SomeInterface
		Sub Something(<SomeAttribute> helloWorld As String)
	End Interface

	Private Class SomeAttribute
		Inherits Attribute
	End Class
End Class",
"Imports System

Public Class Foo
    Implements Holder.SomeInterface

    Public Sub Something(helloWorld As String) Implements Holder.SomeInterface.Something
        Throw New NotImplementedException()
    End Sub
End Class

Public Class Holder
	Public Interface SomeInterface
		Sub Something(<SomeAttribute> helloWorld As String)
	End Interface

	Private Class SomeAttribute
		Inherits Attribute
	End Class
End Class", compareTokens:=False)
        End Function


        <WorkItem(2785, "https://github.com/dotnet/roslyn/issues/2785")>
        <Fact(), Trait(Traits.Feature, Traits.Features.CodeActionsImplementInterface)>
        Public Async Function TestImplementInterfaceThroughStaticMemberInGenericClass() As Task
            Await TestAsync(
"Imports System
Imports System.Collections.Generic
Class Program(Of T)
    Implements [|IList(Of Object)|]
    Private Shared innerList As List(Of Object) = New List(Of Object)
End Class",
"Imports System
Imports System.Collections
Imports System.Collections.Generic
Class Program(Of T)
    Implements IList(Of Object)
    Private Shared innerList As List(Of Object) = New List(Of Object)

    Default Public Property Item(index As Integer) As Object Implements IList(Of Object).Item
        Get
            Return DirectCast(innerList, IList(Of Object))(index)
        End Get
        Set(value As Object)
            DirectCast(innerList, IList(Of Object))(index) = value
        End Set
    End Property

    Public ReadOnly Property Count As Integer Implements ICollection(Of Object).Count
        Get
            Return DirectCast(innerList, IList(Of Object)).Count
        End Get
    End Property

    Public ReadOnly Property IsReadOnly As Boolean Implements ICollection(Of Object).IsReadOnly
        Get
            Return DirectCast(innerList, IList(Of Object)).IsReadOnly
        End Get
    End Property

    Public Sub Insert(index As Integer, item As Object) Implements IList(Of Object).Insert
        DirectCast(innerList, IList(Of Object)).Insert(index, item)
    End Sub

    Public Sub RemoveAt(index As Integer) Implements IList(Of Object).RemoveAt
        DirectCast(innerList, IList(Of Object)).RemoveAt(index)
    End Sub

    Public Sub Add(item As Object) Implements ICollection(Of Object).Add
        DirectCast(innerList, IList(Of Object)).Add(item)
    End Sub

    Public Sub Clear() Implements ICollection(Of Object).Clear
        DirectCast(innerList, IList(Of Object)).Clear()
    End Sub

    Public Sub CopyTo(array() As Object, arrayIndex As Integer) Implements ICollection(Of Object).CopyTo
        DirectCast(innerList, IList(Of Object)).CopyTo(array, arrayIndex)
    End Sub

    Public Function IndexOf(item As Object) As Integer Implements IList(Of Object).IndexOf
        Return DirectCast(innerList, IList(Of Object)).IndexOf(item)
    End Function

    Public Function Contains(item As Object) As Boolean Implements ICollection(Of Object).Contains
        Return DirectCast(innerList, IList(Of Object)).Contains(item)
    End Function

    Public Function Remove(item As Object) As Boolean Implements ICollection(Of Object).Remove
        Return DirectCast(innerList, IList(Of Object)).Remove(item)
    End Function

    Public Function GetEnumerator() As IEnumerator(Of Object) Implements IEnumerable(Of Object).GetEnumerator
        Return DirectCast(innerList, IList(Of Object)).GetEnumerator()
    End Function

    Private Function IEnumerable_GetEnumerator() As IEnumerator Implements IEnumerable.GetEnumerator
        Return DirectCast(innerList, IList(Of Object)).GetEnumerator()
    End Function
End Class",
index:=1)
        End Function

        <WorkItem(11444, "https://github.com/dotnet/roslyn/issues/11444")>
        <Fact, Trait(Traits.Feature, Traits.Features.CodeActionsImplementInterface)>
        Public Async Function TestAbstractConflictingMethod() As Task
            Await TestAsync(
"Friend Interface IFace
    Sub M()
End Interface

Public MustInherit Class C
    Implements [|IFace|]

    Public MustOverride Sub M()
End Class",
"Friend Interface IFace
    Sub M()
End Interface

Public MustInherit Class C
    Implements IFace

    Public MustOverride Sub M()
    Public MustOverride Sub IFace_M() Implements IFace.M
End Class",
index:=1)
        End Function

        <WorkItem(16793, "https://github.com/dotnet/roslyn/issues/16793")>
        <Fact, Trait(Traits.Feature, Traits.Features.CodeActionsImplementInterface)>
        Public Async Function TestMethodWithValueTupleArity1() As Task
            Await TestAsync(
"
Imports System
interface I
    Function F() As ValueTuple(Of Object)
end interface
class C
    Implements [|I|]

end class
Namespace System
    Structure ValueTuple(Of T1)
    End Structure
End Namespace",
"
Imports System
interface I
    Function F() As ValueTuple(Of Object)
end interface
class C 
    Implements I

Public Function F() As ValueTuple(Of Object) Implements I.F
        Throw New NotImplementedException()
    End Function
end class
Namespace System
    Structure ValueTuple(Of T1)
    End Structure
End Namespace")
        End Function
    End Class
End Namespace