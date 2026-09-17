using Xunit;

// Svi integracioni testovi dele istu instancu lokalne baze podataka
// (igraonica_test), pa se izvrsavaju sekvencijalno kako bi se izbegla
// trka pri pokretanju vise WebApplicationFactory instanci koje svaka
// za sebe primenjuju migracije i pocetne podatke.
[assembly: CollectionBehavior(DisableTestParallelization = true)]
