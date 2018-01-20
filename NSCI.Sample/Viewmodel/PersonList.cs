using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NDProperty;
using NSCI.Propertys;

namespace NSCI.Sample.Viewmodel
{
    partial class PersonList
    {

        // This Object uses like the UI NDPropertys.
        // This is necesarry to use Binding. Currently POCO objects are not supported
        // If you wan't to create your own NDPropertys, you should add the 
        // NDProperty nuget package explicitly. This will include buildtime generators
        // in you project that will normaly not added if it will referenced via 
        // another dependency. While you don't need this generator it will make things
        // easier. To add an NDProperty you need to implement a OnChangeing method.
        // for example the CurrentSelectedIndexProeprty has following callback;
        // private void OnCurrentSelectedIndexChanging(NDProperty.Propertys.OnChangingArg<NDPConfiguration, int> arg)
        // The first TypeArgument sets the configuration for the PropertySystem and must be
        // the same as in the NSCI project. The seccond TypeArgument is the Type of the Proeprty.
        // The last thing you need to do is add the [NDP] Attribute to the Method and the rest will
        // be generated.


        private readonly ObservableCollection<Person> persons;
        private readonly FileInfo databaseFile;

        public ReadOnlyObservableCollection<Person> Persons { get; }

        private PersonList(FileInfo databaseFile, IEnumerable<Person> persons)
        {
            this.databaseFile = databaseFile;
            this.persons = new ObservableCollection<Person>(persons);
            Persons = new ReadOnlyObservableCollection<Person>(this.persons);

            this.persons.CollectionChanged += (sender, e) => Update();
            Update();

        }

        private void Update()
        {
            Count = this.persons.Count;
            if (this.persons.Count > 0)
            {
                if (CurrentSelectedIndex < 0)
                    CurrentSelectedIndex = 0;
                Current = this.persons[CurrentSelectedIndex];
                HasNext = CurrentSelectedIndex + 1 < this.persons.Count;
                HasPrevious = CurrentSelectedIndex > 0;
            }
            else
            {
                Current = null;
                HasNext = false;
                HasPrevious = false;
                CurrentSelectedIndex = -1;
            }
        }

        [NDP]
        private void OnCurrentSelectedIndexChanging(NDProperty.Propertys.OnChangingArg<NDPConfiguration, int> arg)
        {
            if (arg.Provider.NewValue >= this.persons.Count || (arg.Provider.NewValue < 0 && this.persons.Count != 0))
                arg.Provider.Reject = true;
            if (arg.Property.IsObjectValueChanging)
                arg.ExecuteAfterChange += (sender, e) =>
                  Update();
        }


        [NDP(Settings = NDProperty.Propertys.NDPropertySettings.ReadOnly)]
        private void OnCurrentChanging(NDProperty.Propertys.OnChangingArg<NDPConfiguration, Person> arg)
        {

        }

        [NDP(Settings = NDProperty.Propertys.NDPropertySettings.ReadOnly)]
        private void OnHasNextChanging(NDProperty.Propertys.OnChangingArg<NDPConfiguration, bool> arg)
        {

        }

        [NDP(Settings = NDProperty.Propertys.NDPropertySettings.ReadOnly)]
        private void OnHasPreviousChanging(NDProperty.Propertys.OnChangingArg<NDPConfiguration, bool> arg)
        {

        }


        [NDP(Settings = NDProperty.Propertys.NDPropertySettings.ReadOnly)]
        private void OnCountChanging(NDProperty.Propertys.OnChangingArg<NDPConfiguration, int> arg)
        {

        }

        public void NewPerson() => this.persons.Add(new Person());



        public static PersonList Load(FileInfo databaseFile)
        {
            PersonList list;
            if (!databaseFile.Exists)
            {
                if (!databaseFile.Directory.Exists)
                    databaseFile.Directory.Create();

                list = new PersonList(databaseFile, Enumerable.Empty<Person>());
                list.Persist();
            }
            else
            {
                var doc = new System.Xml.XmlDocument();
                using (var stream = databaseFile.OpenRead())
                    doc.Load(stream);

                var root = doc.FirstChild;

                var persons = root.ChildNodes.OfType<System.Xml.XmlElement>().Select(x =>
                {
                    return new Person()
                    {
                        FirstName = x.Attributes[nameof(Person.FirstName)].Value,
                        SureName = x.Attributes[nameof(Person.SureName)].Value,
                        Street = x.Attributes[nameof(Person.Street)].Value,
                        City = x.Attributes[nameof(Person.City)].Value
                    };
                });
                list = new PersonList(databaseFile, persons);
            }
            return list;
        }



        public void Persist()
        {
            var doc = new System.Xml.XmlDocument();
            var root = doc.CreateElement(nameof(Persons));
            doc.AppendChild(root);

            foreach (var p in Persons)
            {
                var e = doc.CreateElement(nameof(Person));
                var firstNameAttribute = doc.CreateAttribute(nameof(Person.FirstName));
                var suretNameAttribute = doc.CreateAttribute(nameof(Person.SureName));
                var streetNameAttribute = doc.CreateAttribute(nameof(Person.Street));
                var cityNameAttribute = doc.CreateAttribute(nameof(Person.City));

                firstNameAttribute.Value = p.FirstName;
                suretNameAttribute.Value = p.SureName;
                streetNameAttribute.Value = p.Street;
                cityNameAttribute.Value = p.City;

                e.Attributes.Append(firstNameAttribute);
                e.Attributes.Append(suretNameAttribute);
                e.Attributes.Append(streetNameAttribute);
                e.Attributes.Append(cityNameAttribute);
                root.AppendChild(e);
            }

            using (var stream = this.databaseFile.Open(FileMode.Create, FileAccess.Write, FileShare.None))
                doc.Save(stream);
        }

    }
}
