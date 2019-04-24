--
--    Comme SQLite ne verifie pas les types de donnees,
--    des contraintes de type CHECK ont ete ajoutees ou
--    c'etait necessaire.
--
-- -----------------------------------------------------
-- table personnages
-- -----------------------------------------------------
create table if not exists personnages (
  id_personnage  integer not null primary key, 
  nom_personnage varchar(20) not null
           constraint longueur_nom_personnage
                check (length(nom_personnage) <= 20),
  constraint nom_personnage_unique unique(nom_personnage))
;

-- -----------------------------------------------------
-- table pieces
-- -----------------------------------------------------
create table if not exists pieces (
  id_piece integer not null primary key,
  titre    varchar(30) not null
           constraint longueur_titre
                check (length(titre) <= 30),
  constraint titre_unique unique(titre))
;

-- -----------------------------------------------------
-- table tirades
-- -----------------------------------------------------
create table if not exists tirades (
  id_tirade     integer not null primary key,
  id_piece      int not null,  -- Verification type par FK
  id_personnage int not null,  -- Verification type par FK
  -- Il y a parfois des prologues ou conclusions
  -- en dehors d'un acte et d'une scene, donc
  -- ils peuvent etre "null"
  acte          int
           constraint acte_entier
               check(acte is null
                     or (acte+0=acte  -- verification numerique
                         and round(acte)=acte)),
  scene         int
           constraint scene_entier
               check(scene is null
                     or (scene+0=scene
                         and round(scene)=scene)),
  constraint tirades_fk1
    foreign key (id_piece)
    references pieces (id_piece),
  constraint tirades_fk2
    foreign key (id_personnage)
    references personnages (id_personnage))
;

-- -----------------------------------------------------
-- table texte
-- -----------------------------------------------------
create table if not exists texte (
  id          int not null primary key,
  id_piece    int not null,  -- Verification type par FK
  id_tirade   int,           -- Verification type par FK
  numero_vers int
           constraint numero_vers_entier
               check(numero_vers is null
                     or (numero_vers+0=numero_vers  -- verification numerique
                         and round(numero_vers)=numero_vers)),
  texte        varchar(1500) not null  -- Il y a des passages en prose
                                       -- qui peuvent etre longs
           constraint longueur_texte
                check (length(texte) <= 1500),
  constraint texte_fk1
    foreign key (id_tirade)
    references tirades (id_tirade),
  constraint texte_fk2
    foreign key (id_piece)
    references pieces (id_piece));
