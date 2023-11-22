import { Dropdown, Table } from "react-bootstrap";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faEllipsisVertical } from "@fortawesome/free-solid-svg-icons";
import React from "react";
import Image from "next/image";
import { Category } from "@models/cartegory";
import { THSort } from "@components/TableSort";

const typeColorMap: Record<string, string> = {
  normal: "#aa9",
  fighting: "#b54",
  flying: "#89f",
  poison: "#a59",
  ground: "#db5",
  rock: "#ba6",
  bug: "#ab2",
  ghost: "#66b",
  steel: "#aab",
  fire: "#f42",
  water: "#39f",
  grass: "#7c5",
  electric: "#fc3",
  psychic: "#f59",
  ice: "#6cf",
  dragon: "#76e",
  dark: "#754",
  fairy: "#e9e",
  unknown: "#aa9",
  shadow: "#aa9",
};

type TypeLabelProps = {
  type: string;
};

const TypeLabel = ({ type }: TypeLabelProps) => (
  <span
    className="text-white d-inline-block text-uppercase text-center rounded-1 shadow-sm me-2"
    style={{
      backgroundColor: typeColorMap[type],
      textShadow: "1px 1px 2px rgb(0 0 0 / 70%)",
      fontSize: ".7rem",
      width: "70px",
    }}
  >
    {type}
  </span>
);

type Props = {
  categories: Category[];
} & Pick<Parameters<typeof THSort>[0], "setSort" | "setOrder">;

export default function CategoryList(props: Props) {
  const { categories, setSort, setOrder } = props;

  return (
    <Table responsive bordered hover>
      <thead className="bg-light">
        <tr>
          <th>
            <THSort name="id" setSort={setSort} setOrder={setOrder}>
              #
            </THSort>
          </th>
          <th>
            <THSort name="name" setSort={setSort} setOrder={setOrder}>
              Name
            </THSort>
          </th>
          <th aria-label="Action" />
        </tr>
      </thead>
      <tbody>
        {categories.map((category) => (
          <tr key={category.id}>
            <td>{category.id}</td>
            <td className="text-end">{category.name}</td>
            <td>
              <Dropdown align="end">
                <Dropdown.Toggle
                  as="button"
                  bsPrefix="btn"
                  className="btn-link rounded-0 text-black-50 shadow-none p-0"
                  id={`action-${category.id}`}
                >
                  <FontAwesomeIcon fixedWidth icon={faEllipsisVertical} />
                </Dropdown.Toggle>

                <Dropdown.Menu>
                  <Dropdown.Item href="#/action-1">Info</Dropdown.Item>
                  <Dropdown.Item href="#/action-2">Edit</Dropdown.Item>
                  <Dropdown.Item className="text-danger" href="#/action-3">
                    Delete
                  </Dropdown.Item>
                </Dropdown.Menu>
              </Dropdown>
            </td>
          </tr>
        ))}
      </tbody>
    </Table>
  );
}
